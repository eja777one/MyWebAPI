using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Attributes;
using MyWebAPI.Dto;
using MyWebAPI.Models.Posts;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Dto.Blogs;

namespace MyWebAPI.Controllers
{
    [ApiController]
    [Route("/posts")]
    public class PostsController
    {
        private readonly IPostsService _postsService;
        private readonly IPostsRepository _postsRepository;

        public PostsController(IPostsService postsService, IPostsRepository postsRepository)
        {
            _postsService = postsService;
            _postsRepository = postsRepository;
        }

        /// <summary>Returns all posts</summary>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageNumber">pageNumber is number of portions that should be returned</param>
        /// <param name="pageSize">pageSize is portions size that should be returned</param>

        [HttpGet]
        [ProducesResponseType(typeof(PaginatorDto<Post>), StatusCodes.Status200OK)]
        public async Task<IResult> GetPosts(string sortBy = "createdAt",
            string sortDirection = "desc", int pageNumber = 1, int pageSize = 10)
        {
            var dto = new GetPostsQueryDto(sortBy, sortDirection, pageNumber, pageSize);

            var posts = await _postsRepository.GetPosts(dto);
            return TypedResults.Ok(posts);
        }

        /// <summary>Create new post</summary>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(Post), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult> CreatePost(InputPostDto dto)
        {
            var post = await _postsService.AddPost(dto);
            return post is null ? TypedResults.BadRequest() : TypedResults.Created("", post);
        }

        /// <summary>Returns post by id</summary>
        /// <param name="id">Id of existing post</param>
        /// <response code="404">Not found</response>
        [HttpGet]
        [Route("/posts/{id}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        public async Task<IResult> GetPost(int id)
        {
            var post = await _postsRepository.GetPost(id);
            return post is null ? TypedResults.NotFound() : TypedResults.Ok(post);
        }

        /// <summary>Update an existing post by id with InputModel</summary>
        /// <param name="id">Id of existing post</param>
        /// <response code="204">No content</response>
        /// <response code="400">If the inputModel has incorrect values</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [HttpPut]
        [Route("/posts/{id}")]
        [BasicAuthorization]
        [ValidateModel]
        [ProducesResponseType(typeof(ApiErrorResultDto), StatusCodes.Status400BadRequest)]
        public async Task<IResult> UpdatePost(int id, InputPostDto dto)
        {
            var isUpdated = await _postsService.UpdatePost(id, dto);
            return isUpdated ? TypedResults.NoContent() : TypedResults.NotFound();
        }

        /// <summary>Delete post specified by id</summary>
        /// <param name="id">Id of existing post</param>
        /// <response code="204">No content</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Not Found</response>
        [HttpDelete]
        [BasicAuthorization]
        [Route("/posts/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IResult> DeletePost(int id)
        {
            var isDeleted = await _postsRepository.DeletePost(id);
            return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
        }
    }
}
