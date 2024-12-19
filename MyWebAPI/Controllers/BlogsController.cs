using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Dto;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;
using MyWebAPI.Dto.Blogs;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("/blogs")]
    public class BlogsController
    {
        private readonly IBlogsService _blogsService;
        private readonly IBlogsRepository _blogsRepository;

        public BlogsController(IBlogsService blogsService, IBlogsRepository blogsRepository)
        {
            _blogsService = blogsService;
            _blogsRepository = blogsRepository;
        }

        /// <summary>Returns all blogs</summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<Blog>), StatusCodes.Status200OK)]
        public async Task<IResult> GetBlogs()
        {
            var blogs = await _blogsRepository.GetBlogs();
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
        /// <response code="204">No content</response>
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
