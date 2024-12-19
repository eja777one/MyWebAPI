using MyWebAPI.Dto.Blogs;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Services
{
    public class BlogsService : IBlogsService
    {
        private readonly IBlogsRepository _blogsRepository;

        public BlogsService(IBlogsRepository blogsRepository)
        {
            _blogsRepository = blogsRepository;
        }

        public async Task<Blog?> AddBlog(InputBlogDto dto)
        {
            return await _blogsRepository.AddBlog(new(dto));
        }

        public async Task<bool> UpdateBlog(int id, InputBlogDto dto)
        {
            var blog = await _blogsRepository.GetBlog(id);

            if (blog == null) return false;

            blog.Update(dto);

            return await _blogsRepository.SaveChanges();
        }
    }
}
