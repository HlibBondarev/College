using Microsoft.AspNetCore.Identity;
using College.DAL.Entities.JwtAuthentication;

namespace College.WebApi.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task SeedRoles(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var roleManager = app.GetRequiredService<RoleManager<IdentityRole>>();
        var loggerFactory = app.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();

        var roles = builder.Configuration.GetSection("Authentication").GetSection("Roles").Get<IEnumerable<string>>();

        try
        {
            if (roles is null || !roles.Any())
            {
                string errorMsg = string.Format(
                "There are no roles in the Authentication.Roles section of the appsettings.json file.");
                throw new NullReferenceException(errorMsg);
            }

            foreach (var role in roles!)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, string.Format("An error occurred seeding the DB. {0}", ex.Message));
        }
    }

    public static async Task SeedAdmin(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var userManager = app.GetRequiredService<UserManager<ApplicationUser>>();
        var loggerFactory = app.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();

        string? userName = builder.Configuration.GetSection("Authentication").GetSection("Admin").GetValue<string>("Email");

        try
        {
            if (userName is null)
            {
                string errorMsg = string.Format(
                "The Email parameter in the Authentication.Admin section of the appsettings.json (or secret.json) file is not filled in.");
                throw new NullReferenceException(errorMsg);
            }

            var existingUser = await userManager.FindByNameAsync(userName);

            if (existingUser is null)
            {
                string password = builder.Configuration.GetSection("Authentication").GetSection("Admin").GetValue<string>("Password") ?? string.Empty;
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, builder.Configuration.GetSection("Authentication").GetSection("Roles").Get<List<string>>()![0]);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, string.Format("An error occurred seeding the DB. {0}", ex.Message));
        }
    }
}
