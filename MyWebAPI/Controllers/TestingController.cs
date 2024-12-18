using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Repositories.Interfaces;

namespace MyWebAPI.Controllers
{
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;

        public TestingController(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        /// <summary>Clear DB</summary>
        /// <response code="204">All data is deleted</response>
        [HttpDelete]
        [Route("/testing/all-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeleteAllVideos()
        {
            await _videoRepository.DeleteAllVideos();
            return TypedResults.NoContent();
        }
    }
}
