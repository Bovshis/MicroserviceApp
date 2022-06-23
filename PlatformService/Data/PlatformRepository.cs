using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _db;

        public PlatformRepository(AppDbContext db)
        {
            _db = db;
        }

        public bool SaveChanges() => _db.SaveChanges() >= 0;

        public IEnumerable<Platform> GetAll() => _db.Platforms.ToList();

        public Platform? Get(int id)
        {
            var platform = _db.Platforms.FirstOrDefault(p => p.Id == id);
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            return platform;
        }

        public void Add(Platform platform)
        {
            if (platform == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _db.Platforms.Add(platform);
        }
    }
}
