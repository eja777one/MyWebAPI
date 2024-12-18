using Microsoft.EntityFrameworkCore;
using MyWebAPI.Models.Video;

namespace MyWebAPI.Data
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        public DbSet<Video> Videos { get; set; }
    }
}
