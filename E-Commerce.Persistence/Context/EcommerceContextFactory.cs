using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace E_Commerce.Persistence.Context;

public sealed class EcommerceContextFactory : IDesignTimeDbContextFactory<EcommerceContext>
{
    public EcommerceContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var apiProjectPath = Path.Combine(currentDirectory, "E-Commerce.API");

        if (!Directory.Exists(apiProjectPath))
        {
            apiProjectPath = Path.GetFullPath(Path.Combine(currentDirectory, "..", "E-Commerce.API"));
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetSection("Database:SqlServer:ConnectionString").Value;

        var optionsBuilder = new DbContextOptionsBuilder<EcommerceContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new EcommerceContext(optionsBuilder.Options);
    }
}
