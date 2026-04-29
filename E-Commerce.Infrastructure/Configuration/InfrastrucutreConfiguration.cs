using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Identity;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Infrastructure.Auth.Jwt;
using E_Commerce.Infrastructure.Auth.RefreshTokens;
using E_Commerce.Infrastructure.BackgroundJobs.Hangfire;
using E_Commerce.Infrastructure.Carts;
using E_Commerce.Infrastructure.Common;
using E_Commerce.Infrastructure.Emails;
using E_Commerce.Infrastructure.Identity;
using E_Commerce.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Microsoft.Extensions.Http;
using System;
using Hangfire.AspNetCore;
using Hangfire.SqlServer;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Infrastructure.TotpTwoFactorAuth;

namespace E_Commerce.Infrastructure.Configuration
{
    public static class InfrastrucutreConfiguration
    {
        public static IServiceCollection ApplyInfrastructureConfiguration (this IServiceCollection services , IConfiguration config)
        {
            services.AddOptions<CartSessionOptions>().Bind(config.GetSection("Cart:Session")).ValidateOnStart();
            services.AddOptions<RefreshTokenOptions>().Bind(config.GetSection("Auth:RefreshToken")).ValidateOnStart();
            services.AddOptions<JwtOptions>().Bind(config.GetSection("Auth:Jwt")).ValidateOnStart();
            services.AddOptions<MailTrapProviderOptions>().Bind(config.GetSection("MailTrap")).ValidateOnStart();


            // Jwt
            services.AddSingleton<IJwtTokenService, JwtTokenService>();


            // Refresh tokens
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            // services.AddScoped<IRefreshTokenRepository, EfOrJsonRefreshTokenRepository>();

            // Identity helpers
            services.AddSingleton<IPasswordHasherAdapter, PasswordHasherAdapter>();
            services.AddScoped<IUserAccessor, UserAccessor>();

            // Cart sessions + merge
            services.AddScoped<IAnonymousCartIdCookie, AnonymousCartIdCookie>();
            services.AddScoped<ICartSessionService, CartSessionService>();
            services.AddScoped<ICartMergeService, CartMergeService>();
            services.AddScoped<ITotpHandler, TotpHandler>();


            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<ITokenHasher, TokenHasher>();

            services.AddTransient<IEmailJobService, HangfireEmailJobService>();
            services.AddTransient<IRandomStringGenerator, RandomStringGenerator>();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailTemplateRenderer, EmailTemplateRenderer>();

            services.AddHangfire(cfg =>
            {
                cfg
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(config.GetSection("Database:SqlServer:ConnectionString").Value, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });
            });
            services.AddHangfireServer();
            return services;
        }
    }
}
