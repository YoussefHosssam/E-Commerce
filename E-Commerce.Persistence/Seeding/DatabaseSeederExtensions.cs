using Microsoft.Extensions.DependencyInjection;

namespace E_Commerce.Persistence.Seeding;

public static class DatabaseSeederExtensions
{
    public static async Task SeedDataAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedDataAsync(cancellationToken);
    }
}
