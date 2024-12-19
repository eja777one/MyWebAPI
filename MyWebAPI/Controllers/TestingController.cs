using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Repositories.Interfaces;

namespace MyWebAPI.Controllers
{
    [ApiController]
    public class TestingController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IBlogsRepository _blogsRepository;
        private readonly IPostsRepository _postsRepository;

        public TestingController(IVideoRepository videoRepository, IBlogsRepository blogsRepository,
            IPostsRepository postsRepository)
        {
            _videoRepository = videoRepository;
            _blogsRepository = blogsRepository;
            _postsRepository = postsRepository;
        }

        /// <summary>Clear DB</summary>
        /// <response code="204">All data is deleted</response>
        [HttpDelete]
        [Route("/testing/all-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeleteAllVideos()
        {
            await _videoRepository.DeleteAllVideos();
            await _blogsRepository.DeleteAllBlogs();
            await _postsRepository.DeleteAllPosts();

            return TypedResults.NoContent();
        }
    }
}
