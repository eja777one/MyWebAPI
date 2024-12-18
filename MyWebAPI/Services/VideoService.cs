using MyWebAPI.Data;
using MyWebAPI.Dto.Video;
using MyWebAPI.Models.Video;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _repository;

        public VideoService(IVideoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Video> AddVideo(CreateVideoDto dto)
        {
            var video = await _repository.AddVideo(new(dto));
            return video;
        }

        public async Task<bool> UpdateVideo(int id, UpdateVideoDto dto)
        {
            var video = await _repository.GetVideo(id);

            if (video == null) return false;

            video.Update(dto);

            await _repository.SaveChanges();

            return true;
        }
    }
}
