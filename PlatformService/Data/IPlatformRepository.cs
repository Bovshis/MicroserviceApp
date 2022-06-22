using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepository
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAll();
        Platform Get(int id);
        void Add(Platform platform);
    }
}
