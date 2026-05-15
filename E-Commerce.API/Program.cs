using Asp.Versioning;
using E_Commerce.Infrastructure.Configuration;
using E_Commerce.Infrastructure.Settings;
using E_Commerce.Application.Configuration;
using E_Commerce.Persistence.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_Commerce.API.Middlewares;
using E_Commerce.API.Configuration;
using System.Text.Json.Serialization;
using Serilog;
using Serilog.Events;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId();
});

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});
builder.Services.AddHttpContextAccessor();
builder.Services.ApplyInfrastructureConfiguration(builder.Configuration);
builder.Services.ApplyPersistenceConfiguration(builder.Configuration);
builder.Services.ApplyApplicationConfiguration();
builder.Services.ApplyApiConfiguration(builder.Configuration);

var app = builder.Build();

Log.Information(
    "Application starting in {Environment} environment",
    app.Environment.EnvironmentName);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
    });
}
app.UseMiddleware<CorrelationIdMiddleware>();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    options.GetLevel = (httpContext, elapsed, exception) =>
    {
        if (exception is not null)
            return LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
            return LogEventLevel.Error;

        if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest)
            return LogEventLevel.Warning;

        if (elapsed > 1000)
            return LogEventLevel.Warning;

        return LogEventLevel.Information;
    };

    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("TraceId", httpContext.TraceIdentifier);
        diagnosticContext.Set("UserId", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        diagnosticContext.Set("UserRole", httpContext.User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty);
        diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
        diagnosticContext.Set("EndpointName", httpContext.GetEndpoint()?.DisplayName ?? string.Empty);
    };
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseMiddleware<SerilogContextEnrichmentMiddleware>();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

try
{
    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Application stopped");
    Log.CloseAndFlush();
}
