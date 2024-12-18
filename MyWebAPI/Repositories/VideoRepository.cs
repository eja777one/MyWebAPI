using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Models.Video;
using MyWebAPI.Repositories.Interfaces;

namespace MyWebAPI.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly MainDbContext _context;

        public VideoRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<Video> AddVideo(Video video)
        {
            await _context.Videos.AddAsync(video);
            await _context.SaveChangesAsync();
            return video;
        }

        public async Task<bool> DeleteAllVideos()
        {
            var videos = await _context.Videos.ToListAsync();
            _context.Videos.RemoveRange(videos);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteVideo(int id)
        {
            var video = await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);

            if (video is null) return false;

            _context.Videos.Remove(video);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Video?> GetVideo(int id)
        {
            return await _context.Videos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Video>> GetVideos()
        {
            var videos = await _context.Videos.ToListAsync() ?? new();
            return videos;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
