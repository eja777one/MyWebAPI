using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Dto;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("/blogs")]
    public class BlogsController
    {
        private readonly IBlogsService _blogsService;
        private readonly IBlogsRepository _blogsRepository;
        private readonly IPostsRepository _postsRepository;
        private readonly IPostsService _postsService;

        public BlogsController(IBlogsService blogsService, IBlogsRepository blogsRepository,
            IPostsRepository postsRepository, IPostsService postsService)
        {
            _blogsService = blogsService;
            _blogsRepository = blogsRepository;
            _postsRepository = postsRepository;
            _postsService = postsService;
        }

        /// <summary>Returns all blogs</summary>
        /// <param name="searchNameTerm">Search term for blog Name: Name should contains this term in any position</param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber">pageNumber is number of portions that should be returned</param>
        /// <param name="pageSize">pageSize is portions size that should be returned</param>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatorDto<Blog>), StatusCodes.Status200OK)]
        public async Task<IResult> GetBlogs(string searchNameTerm = "null", string sortBy = "createdAt",
            string sortDirection = "desc", int pageNumber = 1, int pageSize = 10)
        {
            var dto = new GetBlogsQueryDto(searchNameTerm, sortBy, sortDirection, pageNumber, pageSize);

            var blogs = await _blogsRepository.GetBlogs(dto);
            return TypedResults.Ok(blogs);
        }

        /// <summary>Create new blog</summary>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(Blog), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult?> CreateBlog(InputBlogDto dto)
        {
            var blog = await _blogsService.AddBlog(dto);
            return blog is null ? TypedResults.BadRequest() : TypedResults.Created("", blog);
        }

        /// <summary>Returns all posts for specified blog</summary>
        /// <param name="blogId">Id of existing blog</param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber">pageNumber is number of portions that should be returned</param>
        /// <param name="pageSize">pageSize is portions size that should be returned</param>
        /// <response code="404">If specificied blog doesn't exist</response>
        [HttpGet]
        [Route("/blogs/{blogId}/posts")]
        [ProducesResponseType(typeof(PaginatorDto<Blog>), StatusCodes.Status200OK)]
        public async Task<IResult?> GetPostsForBlog(int blogId, string sortBy = "createdAt",
            string sortDirection = "desc", int pageNumber = 1, int pageSize = 10)
        {
            var dto = new GetPostsQueryDto(sortBy, sortDirection, pageNumber, pageSize);

            var posts = await _postsRepository.GetPostsForBlog(blogId, dto);
            return posts != null ? TypedResults.Ok(posts) : TypedResults.NotFound();
        }

        /// <summary>Create new post for specific blog</summary>
        /// <param name="blogId">Id of existing blog</param>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">If specificied blog doesn't exist</response>
        [HttpPost]
        [Route("/blogs/{blogId}/posts")]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(Post), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult?> CreatePostForBlog(int blogId, InputBlogPostDto dto)
        {
            var post = await _postsService.AddPostForBlog(blogId, dto);
            return post is null ? TypedResults.NotFound() : TypedResults.Created("", post);
        }

        /// <summary>Returns blog by id</summary>
        /// <param name="id">Id of existing blog</param>
        /// <response code="404">Not found</response>
        [HttpGet]
        [Route("/blogs/{id}")]
        [ProducesResponseType(typeof(Blog), StatusCodes.Status200OK)]
        public async Task<IResult> GetBlog(int id)
        {
            var blog = await _blogsRepository.GetBlog(id);
            return blog is null ? TypedResults.NotFound() : TypedResults.Ok(blog);
        }

        /// <summary>Update an existing blog by id with InputModel</summary>
        /// <param name="id">Id of existing blog</param>
        /// <response code="204">No content</response>
        /// <response code="400">If the inputModel has incorrect values</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [HttpPut]
        [Route("/blogs/{id}")]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult> UpdateBlog(int id, InputBlogDto dto)
        {
            var isUpdated = await _blogsService.UpdateBlog(id, dto);
            return isUpdated ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        /// <summary>Delete blog specified by id</summary>
        /// <param name="id">Id of existing blog</param>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [HttpDelete]
        [BasicAuthorization]
        [Route("/blogs/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeleteBlog(int id)
        {
            var isDeleted = await _blogsRepository.DeleteBlog(id);
            return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
