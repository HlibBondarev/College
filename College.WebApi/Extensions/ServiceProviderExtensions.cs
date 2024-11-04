using Microsoft.AspNetCore.Identity;
using College.DAL.Entities.JwtAuthentication;

namespace College.WebApi.Extensions;

public static class ServiceProviderExtensions
{
    private const string AUTHENTICATION = "Authentication";

    public static async Task SeedRoles(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var roleManager = app.GetRequiredService<RoleManager<IdentityRole>>();
        var loggerFactory = app.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();

        var roles = builder.Configuration.GetSection(AUTHENTICATION).GetSection("Roles").Get<IEnumerable<string>>();

        try
        {
            if (roles is null || !roles.Any())
            {
                string errorMsg = "There are no roles in the Authentication.Roles section of the appsettings.json file.";
                throw new InvalidOperationException(errorMsg);
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
            logger.LogError(ex, "An error occurred seeding the DB. {errMessage}", ex.Message);
        }
    }

    public static async Task SeedAdmin(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var userManager = app.GetRequiredService<UserManager<ApplicationUser>>();
        var loggerFactory = app.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();

        string? userName = builder.Configuration.GetSection(AUTHENTICATION).GetSection("Admin").GetValue<string>("Email");

        try
        {
            if (userName is null)
            {
                string errorMsg = "The Email parameter in the Authentication.Admin section of the appsettings.json (or secret.json) file is not filled in.";
                logger.LogError("The Email parameter in the Authentication.Admin section of the appsettings.json (or secret.json) file is not filled in.");
                throw new InvalidOperationException(errorMsg);
            }

            var existingUser = await userManager.FindByNameAsync(userName);

            if (existingUser is null)
            {
                string password = builder.Configuration.GetSection(AUTHENTICATION).GetSection("Admin").GetValue<string>("Password") ?? string.Empty;
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, builder.Configuration.GetSection(AUTHENTICATION).GetSection("Roles").Get<List<string>>()![0]);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the DB. {errMessage}", ex.Message);
        }
    }
}
