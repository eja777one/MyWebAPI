using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Services
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository _postsRepository;
        private readonly IBlogsRepository _blogsRepository;

        public PostsService(IPostsRepository postsRepository, IBlogsRepository blogsRepository)
        {
            _postsRepository = postsRepository;
            _blogsRepository = blogsRepository;
        }

        public async Task<Post?> AddPost(InputPostDto dto)
        {
            var blog = await _blogsRepository.GetBlog((int)dto.BlogId);
            if (blog is null) return null;

            return await _postsRepository.AddPost(new(dto, blog.Name));
        }

        public async Task<Post?> AddPostForBlog(int blogId, InputBlogPostDto dto)
        {
            var inputPostDto = new InputPostDto(dto.Title, dto.ShortDescription, dto.Content, blogId);
            return await AddPost(inputPostDto);
        }

        public async Task<bool> UpdatePost(int id, InputPostDto dto)
        {
            var post = await _postsRepository.GetPost(id);
            var blog = await _blogsRepository.GetBlog((int)dto.BlogId);

            if (post == null || blog == null) return false;

            post.Update(dto, blog.Name);

            return await _postsRepository.SaveChanges();
        }
    }
}
