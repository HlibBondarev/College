using Microsoft.AspNetCore.Identity;
using College.DAL.Entities.JwtAuthentication;

namespace College.WebApi.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task SeedRoles(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var roleManager = app.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = builder.Configuration.GetSection("Authentication").GetSection("Roles").Get<IEnumerable<string>>();

        foreach (var role in roles!)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdmin(this IServiceProvider app, WebApplicationBuilder builder)
    {
        var userManager = app.GetRequiredService<UserManager<ApplicationUser>>();

        string userName = builder.Configuration.GetSection("Authentication").GetSection("Admin").GetValue<string>("Email")!;
        string password = builder.Configuration.GetSection("Authentication").GetSection("Admin").GetValue<string>("Password")!;

        var existingUser = await userManager.FindByNameAsync(userName!);

        if (existingUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName
            };

            var result = await userManager.CreateAsync(user, password!);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, builder.Configuration.GetSection("Authentication").GetSection("Roles").Get<List<string>>()![0]);
            }
        }
    }
}
