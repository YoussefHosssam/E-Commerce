using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
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
using E_Commerce.Infrastructure.Images;
using E_Commerce.Infrastructure.Payment.Paymob;
using E_Commerce.Infrastructure.Settings;
using E_Commerce.Infrastructure.TotpTwoFactorAuth;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

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
            services.AddOptions<PaymobPyamentOptions>().Bind(config.GetSection("Payment:Paymob")).ValidateOnStart();
            services.AddOptions<ImageStorageOptions>().Bind(config.GetSection("Cloudinary")).ValidateOnStart();

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
            services.AddScoped<IPaymentGateway, PaymobPaymentGateway>();
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();


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
            services.AddHttpClient<PaymobClient>(client =>
            {
                client.BaseAddress = new Uri("https://accept.paymob.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string secretKey = config.GetSection("Payment:Paymob:SecretKey").Value!;
                string authToken = $"Token {secretKey}";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", authToken);
            }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 20,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2)
            }).AddResilienceHandler("paymobPipeline", (pipeline) =>
            {
                pipeline.AddTimeout(TimeSpan.FromSeconds(8));
                pipeline.AddRetry(new()
                {
                    UseJitter = true,
                    BackoffType = DelayBackoffType.Exponential,
                    MaxRetryAttempts = 3,
                });
                pipeline.AddTimeout(TimeSpan.FromSeconds(2));
                pipeline.AddCircuitBreaker(new()
                {
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    FailureRatio = 0.5,
                    BreakDuration = TimeSpan.FromSeconds(5),
                    MinimumThroughput = 6,
                });
            });
            return services;
        }
    }
}
