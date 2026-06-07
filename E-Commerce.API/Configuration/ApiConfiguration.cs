using Asp.Versioning;
using E_Commerce.API.Common.Errors;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Filters;
using E_Commerce.API.Identity;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace E_Commerce.API.Configuration
{
    public static class ApiConfiguration
    {
        public static IServiceCollection ApplyApiConfiguration (this IServiceCollection services , IConfiguration configuration)
        {
            // ---- AUTH ----
            var jwt = configuration.GetSection("Auth:Jwt").Get<JwtOptions>()!;

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false; // dev only
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwt.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                });
            services.AddOpenApi();
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1);
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = new QueryStringApiVersionReader();
            }).AddMvc();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Commerce API",
                    Version = "v1",
                    Description = "API documentation for E-Commerce project"
                });
            });
            services.Configure<ApiBehaviorOptions>(cfg =>
            {
                cfg.InvalidModelStateResponseFactory = ctx =>
                {
                    var jsonError = ctx.ModelState
                        .FirstOrDefault(e => e.Value?.Errors.Any(x => x.Exception is JsonException) == true);

                    if (!jsonError.Equals(default(KeyValuePair<string, ModelStateEntry?>)))
                    {
                        var jsonErrorCode = ModelStateApiErrors.UnsupportedFormat;

                        return ApiResult.Fail(
                            StatusCodes.Status400BadRequest,
                            jsonErrorCode.Code,
                            jsonErrorCode.Message);
                    }

                    var firstError = ctx.ModelState.Values
                        .SelectMany(v => v.Errors)
                        .FirstOrDefault()?.ErrorMessage;

                    var validationErrorCode = ModelStateApiErrors.ValidationError;

                    return ApiResult.Fail(
                        StatusCodes.Status400BadRequest,
                        validationErrorCode.Code,
                        $"{validationErrorCode.Message} - {firstError}");
                };
            });
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IdempotencyFilter>();
            return services;
        }
    }
}
