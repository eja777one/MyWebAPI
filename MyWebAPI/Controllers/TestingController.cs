using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
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

        /// <summary>Add blogs</summary>
        /// <response code="200">Ok</response>
        /// <response code="400">Error</response>
        [HttpPost]
        [Route("/testing/blogs")]
        [BasicAuthorization]
        public async Task<IResult> AddBlogs(List<Blog> blogs)
        {
            var blogsDb = await _blogsRepository.AddBlogs(blogs);
            return blogsDb is null ? TypedResults.BadRequest() : TypedResults.Ok();
        }

        /// <summary>Add posts</summary>
        /// <response code="200">Ok</response>
        /// <response code="400">Error</response>
        [HttpPost]
        [Route("/testing/posts")]
        [BasicAuthorization]
        public async Task<IResult> AddPosts(List<Post> posts)
        {
            var postsDb = await _postsRepository.AddPosts(posts);
            return postsDb is null ? TypedResults.BadRequest() : TypedResults.Ok();
        }
    }
}
