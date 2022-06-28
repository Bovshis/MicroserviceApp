using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly AppDbContext _context;

        public PlatformRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool SaveChanges() => _context.SaveChanges() >= 0;

        public IEnumerable<Platform> GetAll() => _context.Platforms.ToList();

        public Platform? Get(int id)
        {
            var platform = _context.Platforms.FirstOrDefault(p => p.Id == id);
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

            _context.Platforms.Add(platform);
        }
    }
}
