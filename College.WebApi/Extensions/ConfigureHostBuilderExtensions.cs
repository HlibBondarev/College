using Serilog;

namespace College.WebApi.Extensions;

public static class ConfigureHostBuilderExtensions
{
    public static void ConfigureApplication(this ConfigureHostBuilder host)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        host.ConfigureAppConfiguration((_, config) =>
        {
            config.ConfigureCustom(environment);
        });
    }

    public static void ConfigureSerilog(this IServiceCollection services, WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration);
        });
    }
}
