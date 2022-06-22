using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PrepDbConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.HasData(
                new Platform(){Id = 1, Name = "Dot Net", Publisher = "Microsoft", Cost = "Free"}, 
                new Platform(){Id = 1, Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free"}, 
                new Platform(){Id = 1, Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"}
                );
        }
    }
}
