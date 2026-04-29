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


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.ApplyInfrastructureConfiguration(builder.Configuration);
builder.Services.ApplyPersistenceConfiguration(builder.Configuration);
builder.Services.ApplyApplicationConfiguration();
builder.Services.ApplyApiConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();
