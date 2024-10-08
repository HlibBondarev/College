﻿using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using MediatR;
using StackExchange.Redis;
using College.BLL.Behaviors;
using College.BLL.Interfaces;
using College.BLL.Interfaces.Logging;
using College.BLL.MediatR.Teacher.Create;
using College.BLL.Services.JwtAuthentication;
using College.BLL.Services.JwtAuthentication.Settings;
using College.DAL.Entities.JwtAuthentication;
using College.DAL.Persistence;
using College.DAL.Repositories.Interfaces.Base;
using College.DAL.Repositories.Realizations.Base;
using College.Redis.Interfaces;
using College.Redis;
using College.Redis.Models;
using College.BLL.Services.DraftStorage.Interfaces;
using College.BLL.Services.DraftStorage;

namespace College.WebApi.Extensions;

public static class ServiceExtensions
{
    private const string TOKENVALIDATIONPARAMETERS = "TokenValidationParameters";

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

        //User Manager Service
        services.AddScoped<IUserService, UserService>();

        // Caching in Redis
        services.AddSingleton<ICacheService, CacheService>();

        // Draft storage service
        services.AddSingleton(typeof(IDraftStorageService<>), typeof(DraftStorageService<>));
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

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });

            opt.CustomSchemaIds(x => x.FullName);

            opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void ConfigureRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var isRedisEnabled = configuration.GetValue<bool>("Redis:Enabled");
        var redisConnection = $"{configuration.GetValue<string>("Redis:Server")}:{configuration.GetValue<int>("Redis:Port")},password={configuration.GetValue<string>("Redis:Password")}";
        var signalRBuilder = services.AddSignalR();

        if (isRedisEnabled)
        {
            signalRBuilder.AddStackExchangeRedis(redisConnection, options =>
            {
                options.Configuration.AbortOnConnectFail = false;
                options.ConnectionFactory = async writer =>
                {
                    var connection = await ConnectionMultiplexer.ConnectAsync(options.Configuration, writer);
                    return connection;
                };
            });
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
            options.ConnectionMultiplexerFactory = async () =>
            {
                var connection = await ConnectionMultiplexer.ConnectAsync(redisConnection);
                return connection;
            };
        });

        // Redis options
        services.AddOptions<RedisConfig>()
            .Bind(configuration.GetSection(RedisConfig.Name))
            .ValidateDataAnnotations();

        // MemoryCache options
        services.AddOptions<MemoryCacheConfig>()
            .Bind(configuration.GetSection(MemoryCacheConfig.Name))
            .ValidateDataAnnotations();
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

    public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<CollegeDbContext>();

        // JWT options
        services.AddOptions<JwtConfig>()
            .Bind(configuration.GetSection(JwtConfig.Name))
            .ValidateDataAnnotations();

        // Authentication options
        services.AddOptions<AuthenticationConfig>()
            .Bind(configuration.GetSection(AuthenticationConfig.Name))
            .ValidateDataAnnotations();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = configuration.GetSection(TOKENVALIDATIONPARAMETERS).GetValue<bool>("ValidateIssuer"),
            ValidateAudience = configuration.GetSection(TOKENVALIDATIONPARAMETERS).GetValue<bool>("ValidateAudience"),
            ValidateLifetime = configuration.GetSection(TOKENVALIDATIONPARAMETERS).GetValue<bool>("ValidateLifetime"),
            ValidateIssuerSigningKey = configuration.GetSection(TOKENVALIDATIONPARAMETERS).GetValue<bool>("ValidateIssuerSigningKey"),

            ValidIssuer = configuration.GetSection(JwtConfig.Name).GetValue<string>("Issuer"),
            ValidAudience = configuration.GetSection(JwtConfig.Name).GetValue<string>("Audience"),

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection(JwtConfig.Name).GetValue<string>("Key")!))
        };
    });
    }
}
