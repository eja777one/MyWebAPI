using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Video;
using MyWebAPI.Models.Video;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Controllers
{
    [ApiController]
    public class VideosController
    {
        //private readonly ILogger<WeatherForecastController> _logger;
        private readonly IVideoService _videoService;
        private readonly IVideoRepository _videoRepository;

        public VideosController(/*ILogger<WeatherForecastController> logger,*/ IVideoService videoService,
             IVideoRepository videoRepository)
        {
            //_logger = logger;
            _videoService = videoService;
            _videoRepository = videoRepository;
        }

        /// <summary>Returns all videos</summary>
        [HttpGet]
        [Route("/videos")]
        [ProducesResponseType(typeof(List<Video>), StatusCodes.Status200OK)]
        public async Task<IResult> GetVideos()
        {
            var videos = await _videoRepository.GetVideos();
            return TypedResults.Ok(videos);
        }

        /// <summary>Return video by id</summary>
        /// <param name="id">Id of existing video</param>
        /// <response code="404">If video for passed id doesn't exist</response>
        [HttpGet]
        [Route("/videos/{id}")]
        [ProducesResponseType(typeof(Video), StatusCodes.Status200OK)]
        public async Task<IResult> GetVideo(int id)
        {
            var video = await _videoRepository.GetVideo(id);
            return video is null ? TypedResults.NotFound() : TypedResults.Ok(video);
        }

        /// <summary>Create new video</summary>
        [HttpPost]
        [Route("/videos")]
        [ValidateModelAttribute]
        [ProducesResponseType(typeof(Video), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult?> CreateVideo(CreateVideoDto dto)
        {
            var video = await _videoService.AddVideo(dto);
            return video is null ? TypedResults.BadRequest() : TypedResults.Created("", video);
        }

        /// <summary>Update existing video by id with InputModel</summary>
        /// <param name="id">Id of existing video</param>
        /// <response code="204">No content</response>
        /// <response code="400">If the inputModel has incorrect values</response>
        /// <response code="404">Not Found</response>
        [HttpPut]
        [Route("/videos/{id}")]
        [ValidateModelAttribute]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult> UpdateVideo(int id, UpdateVideoDto dto)
        {
            var isUpdated = await _videoService.UpdateVideo(id, dto);
            return isUpdated ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        /// <summary>Delete video specified by id</summary>
        /// <param name="id">Id of existing video</param>
        /// <response code="204">No content</response>
        /// <response code="404">Not Found</response>
        [HttpDelete]
        [Route("/videos/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeleteVideo(int id)
        {
            var isDeleted = await _videoRepository.DeleteVideo(id);
            return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
