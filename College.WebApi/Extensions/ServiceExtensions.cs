using College.BLL.Interfaces.Logging;
using College.BLL.Interfaces;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Base;
using College.DAL.Repositories.Realizations.Base;
using Microsoft.EntityFrameworkCore;
using MediatR;
using College.BLL.Behaviors;
using FluentValidation;
using College.BLL.MediatR.Teacher.Create;


namespace College.WebApi.Extensions;

public static class ServiceExtensions
{
    public static void AddCustomServices(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(currentAssemblies);
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddScoped<ILoggerService, LoggerService>();

        services.AddValidatorsFromAssembly(typeof(CreateTeacherCommandValidator).Assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(LoggerService).Assembly);
            cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString;
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
        if (environment == "Local" || environment == "Development")
        {
            var connection = configuration.GetSection("Local").GetSection("ConnectionStrings").GetValue<string>("DefaultConnection");
            connectionString = connection ?? throw new InvalidOperationException($"'DefaultConnection' is null or not found for the '{environment}' environment.");
        }
        else
        {
            var connection = configuration.GetConnectionString("DefaultConnection");
            connectionString = connection ?? throw new InvalidOperationException("'DefaultConnection' is null or not found");
        }

        services.AddDbContext<CollegeDbContext>(o => o.UseMySql(connectionString,
            MySqlServerVersion.LatestSupportedServerVersion));
    }
}
