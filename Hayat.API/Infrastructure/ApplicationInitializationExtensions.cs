using Hayat.BLL.Interfaces;
using Hayat.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Hayat.API.Infrastructure
{
    public static class ApplicationInitializationExtensions
    {
        public static async Task ApplyDevelopmentDatabaseSetupAsync(this WebApplication application)
        {
            using var scope = application.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HayatDbContext>();
            var seeder = scope.ServiceProvider.GetRequiredService<IDevelopmentDataSeeder>();

            await dbContext.Database.MigrateAsync();
            await seeder.SeedAsync();
        }
    }
}
