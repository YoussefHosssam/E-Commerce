using E_Commerce.Application.Common.Options;
using E_Commerce.Application.Contracts.API.Identity;
using E_Commerce.Application.Contracts.Infrastructure.BackgroundJobs;
using E_Commerce.Application.Contracts.Infrastructure.Cache;
using E_Commerce.Application.Contracts.Infrastructure.Common;
using E_Commerce.Application.Contracts.Infrastructure.Emails;
using E_Commerce.Application.Contracts.Infrastructure.Idempotency;
using E_Commerce.Application.Contracts.Infrastructure.Images;
using E_Commerce.Application.Contracts.Infrastructure.Payment;
using E_Commerce.Application.Contracts.Infrastructure.Shipment;
using E_Commerce.Application.Contracts.Infrastructure.TotpTwoFactorAuth;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.Jwt;
using E_Commerce.Application.Contracts.Infrastrucuture.Auth.RefreshTokens;
using E_Commerce.Application.Contracts.Infrastrucuture.Cart;
using E_Commerce.Application.Features.ImageUploads.Common;
using E_Commerce.Infrastructure.Auth.Jwt;
using E_Commerce.Infrastructure.Auth.RefreshTokens;
using E_Commerce.Infrastructure.BackgroundJobs.Hangfire;
using E_Commerce.Infrastructure.Cache;
using E_Commerce.Infrastructure.Carts;
using E_Commerce.Infrastructure.Common;
using E_Commerce.Infrastructure.Emails;
using E_Commerce.Infrastructure.Idempotency;
using E_Commerce.Infrastructure.Identity;
using E_Commerce.Infrastructure.Images;
using E_Commerce.Infrastructure.Payment.Paymob;
using E_Commerce.Infrastructure.Settings;
using E_Commerce.Infrastructure.Shipment.Bosta;
using E_Commerce.Infrastructure.Shipment.Bosta.Contracts;
using E_Commerce.Infrastructure.TotpTwoFactorAuth;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Serilog;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Configuration
{
    public static class InfrastrucutreConfiguration
    {
        public static IServiceCollection ApplyInfrastructureConfiguration(this IServiceCollection services,IConfiguration config)
        {

            services.RegisterOptions(config);
            services.RegisterCoreServices();
            services.RegisterHangfire(config);
            services.RegisterHttpClients(config);
            services.RegisterResiliencePipelines();

            return services;
        }
        private static void RegisterOptions(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<CartOptions>().Bind(config.GetSection("Cart:Session")).ValidateOnStart();
            services.AddOptions<RefreshTokenOptions>().Bind(config.GetSection("Auth:RefreshToken")).ValidateOnStart();
            services.AddOptions<JwtOptions>().Bind(config.GetSection("Auth:Jwt")).ValidateOnStart();
            services.AddOptions<MailTrapOptions>().Bind(config.GetSection("MailTrap")).ValidateOnStart();
            services.AddOptions<PaymobOptions>().Bind(config.GetSection("Payment:Paymob")).ValidateOnStart();
            services.AddOptions<ImageStorageOptions>().Bind(config.GetSection("Cloudinary")).ValidateOnStart();
            services.AddOptions<BostaOptions>().Bind(config.GetSection("Shipment:Bosta")).ValidateOnStart();
            services.AddOptions<RedisOptions>().Bind(config.GetSection("Cache:Redis")).ValidateOnStart();

        }

        private static void RegisterCoreServices(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<IPasswordHasherAdapter, PasswordHasherAdapter>();
            services.AddSingleton<ITokenHasher, TokenHasher>();

            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAnonymousCartIdCookie, AnonymousCartIdCookie>();
            services.AddScoped<ICartSessionService, CartSessionService>();
            services.AddScoped<ITotpHandler, TotpHandler>();
            services.AddScoped<IPaymentGateway, PaymobGateway>();
            services.AddScoped<IImageStorageService, CloudinaryImageStorageService>();
            services.AddScoped<IImageUploadValidationService, ImageUploadValidationService>();
            services.AddScoped<IShipmentProvider, BostaShipmentProvider>();
            services.AddScoped<IDistributedCacheService, RedisCacheService>();
            services.AddScoped<ILocalCacheService, MemoryCacheService>();
            services.AddScoped<IBostaTokenService, BostaTokenService>();
            services.AddScoped<IIdempotencyStore, IdempotencyStore>();


            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var redisOptions = sp.GetRequiredService<IOptions<RedisOptions>>().Value;

                var options = new ConfigurationOptions
                {
                    EndPoints = { $"{redisOptions.Host}:{redisOptions.Port}" },
                    User = redisOptions.Username,
                    Password = redisOptions.Password,
                    AbortOnConnectFail = false
                };

                return ConnectionMultiplexer.Connect(options);
            });

            services.AddScoped<IDistributedCacheService, RedisCacheService>();

            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddTransient<IEmailQueueService, HangfireEmailQueueService>();
            services.AddTransient<IRandomStringGenerator, RandomStringGenerator>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailTemplateRenderer, EmailTemplateRenderer>();
            services.AddTransient<BostaAuthHandler>();
        }

        private static void RegisterHangfire(this IServiceCollection services, IConfiguration config)
        {
            services.AddHangfire(cfg =>
            {
                cfg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UseSqlServerStorage(
                       config.GetSection("Database:SqlServer:ConnectionString").Value,
                       new SqlServerStorageOptions
                       {
                           CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                           SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                           QueuePollInterval = TimeSpan.Zero,
                           UseRecommendedIsolationLevel = true,
                           DisableGlobalLocks = true
                       });
            });

            services.AddHangfireServer();
        }
        private static void RegisterHttpClients(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<PaymobClient>(client =>
            {
                client.BaseAddress = new Uri("https://accept.paymob.com");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var secretKey = config.GetSection("Payment:Paymob:SecretKey").Value!;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", secretKey);
            })
            .ConfigurePrimaryHttpMessageHandler(CreateSocketsHandler)
            .AddResilienceHandler("paymobPipeline", AddHttpResilience);

            services.AddHttpClient<IBostaTokenService, BostaTokenService>(client =>
            {
                client.BaseAddress = new Uri("https://app.bosta.co");
            });

            services.AddHttpClient<BostaClient>(client =>
            {
                client.BaseAddress = new Uri("https://app.bosta.co");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddHttpMessageHandler<BostaAuthHandler>()
            .ConfigurePrimaryHttpMessageHandler(CreateSocketsHandler)
            .AddResilienceHandler("bostaPipeLine", AddHttpResilience);
        }
        private static SocketsHttpHandler CreateSocketsHandler()
        {
            return new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 20,
                PooledConnectionLifetime = TimeSpan.FromMinutes(5),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2)
            };
        }

        private static void AddHttpResilience(ResiliencePipelineBuilder<HttpResponseMessage> pipeline)
        {
            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                UseJitter = true,
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3,
                ShouldHandle = ShouldRetryHttp
            });

            pipeline.AddTimeout(TimeSpan.FromSeconds(8));

            pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
            {
                SamplingDuration = TimeSpan.FromSeconds(10),
                FailureRatio = 0.5,
                BreakDuration = TimeSpan.FromSeconds(5),
                MinimumThroughput = 6,
            });
        }

        private static ValueTask<bool> ShouldRetryHttp(RetryPredicateArguments<HttpResponseMessage> args)
        {
            if (args.Outcome.Exception is HttpRequestException or TimeoutRejectedException)
                return ValueTask.FromResult(true);

            var response = args.Outcome.Result;

            if (response is null)
                return ValueTask.FromResult(false);

            var shouldRetry =
                response.StatusCode == HttpStatusCode.RequestTimeout ||
                response.StatusCode == HttpStatusCode.TooManyRequests ||
                (int)response.StatusCode >= 500;

            return ValueTask.FromResult(shouldRetry);
        }
        private static void RegisterResiliencePipelines(this IServiceCollection services)
        {
            services.AddResiliencePipeline("cloudinary", builder =>
            {
                builder
                    .AddRetry(CreateDefaultRetryOptions(3))
                    .AddTimeout(TimeSpan.FromSeconds(10))
                    .AddCircuitBreaker(CreateDefaultCircuitBreakerOptions());
            });

            services.AddResiliencePipeline("emailJob", builder =>
            {
                builder
                    .AddRetry(CreateDefaultRetryOptions(2))
                    .AddTimeout(TimeSpan.FromSeconds(10))
                    .AddCircuitBreaker(CreateDefaultCircuitBreakerOptions());
            });
        }

        private static RetryStrategyOptions CreateDefaultRetryOptions(int maxRetryAttempts)
        {
            return new RetryStrategyOptions
            {
                MaxRetryAttempts = maxRetryAttempts,
                Delay = TimeSpan.FromMilliseconds(500),
                UseJitter = true,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<Exception>()
            };
        }

        private static CircuitBreakerStrategyOptions CreateDefaultCircuitBreakerOptions()
        {
            return new CircuitBreakerStrategyOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 10,
                BreakDuration = TimeSpan.FromSeconds(20)
            };
        }
    }
}