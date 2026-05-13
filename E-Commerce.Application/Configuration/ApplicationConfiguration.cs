using E_Commerce.Application.Behaviors;
using E_Commerce.Application.Contracts.Services;
using E_Commerce.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Configuration
{
    public static class ApplicationConfiguration
    {
        public static IServiceCollection ApplyApplicationConfiguration(this IServiceCollection services)
        {
            services.AddAutoMapper((cfg) => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IVerificationEmailPreparationService), typeof(VerificationEmailPreparationService));
            services.AddScoped<IPasswordResetEmailPreparationService, PasswordResetEmailPreparationService>();
            services.AddScoped(typeof(IGenerateLoginTokens), typeof(GenerateLoginTokens));
            services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton<OrderNumberGenerator>();

            return services;
        }
    }
}
