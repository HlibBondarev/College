using College.DAL.Persistence;
using Microsoft.EntityFrameworkCore;

namespace College.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            var streetcodeContext = app.Services.GetRequiredService<CollegeDbContext>();
            await streetcodeContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during startup migration");
        }
    }
}