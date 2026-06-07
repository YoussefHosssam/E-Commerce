using Serilog;

namespace E_Commerce.API.Configuration
{
    public static class LoggingConfiguration
    {
        public static void ConfigureBootstrapLogger(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateBootstrapLogger();
        }

        public static void UseApplicationSerilog(this IHostBuilder host)
        {
            host.UseSerilog((context, services, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId();
            });
        }
    }
}
