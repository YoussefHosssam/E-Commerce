using E_Commerce.Application.Contracts.Persistence;
using E_Commerce.Application.Contracts.Persistence.Shared;
using E_Commerce.Persistence.Context;
using E_Commerce.Persistence.Repositories;
using E_Commerce.Persistence.Repositories.Shared;
using E_Commerce.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Persistence.Configuration;

public static class PersistenceConfiguration
{
    public static IServiceCollection ApplyPersistenceConfiguration(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<EcommerceContext>(opt =>
            opt.UseSqlServer(config.GetSection("Database:SqlServer:ConnectionString").Value , sql =>
            {
                sql
                .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5) , null)
                .CommandTimeout(30);
            }));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IVariantRepository, VariantRepository>();
        services.AddScoped<IAuthTokenRepository, AuthTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentAttemptRepository, PaymentAttemptRepository>();
        services.AddScoped<ITransactionManager, EfTransactionManager>();
        services.AddScoped<DatabaseSeeder>();



        return services;
    }
}
