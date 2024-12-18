using MyWebAPI.Dto.Video;
using MyWebAPI.Models.Video;

namespace MyWebAPI.Services.Interfaces
{
    public interface IVideoService
    {
        Task<Video> AddVideo(CreateVideoDto dto);
        Task<bool> UpdateVideo(int id, UpdateVideoDto dto);
    }
}
