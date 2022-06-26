using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!, isProduction);
        }

        private static void SeedData(AppDbContext db, bool isProduction)
        {
            if (isProduction)
            {
                Console.WriteLine("--> Attempting to apply migrations...");
                try
                {
                    db.Database.Migrate();
                    Console.WriteLine("--> Done migrations!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Could not run migration: {e.Message}");
                }
            }

            if (!db.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...");

                db.Platforms.AddRange(
                    new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
                    new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );

                db.SaveChanges();
            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}
