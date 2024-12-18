using MyWebAPI.Models.Video;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IVideoRepository
    {
        Task<List<Video>> GetVideos();
        Task<Video?> GetVideo(int id);
        Task<Video> AddVideo(Video video);
        Task<bool> DeleteVideo(int id);
        Task<bool> DeleteAllVideos();
        Task SaveChanges();
    }
}
