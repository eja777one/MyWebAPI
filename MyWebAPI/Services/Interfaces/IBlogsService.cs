using MyWebAPI.Dto.Blogs;
using MyWebAPI.Models.Blogs;

namespace MyWebAPI.Services.Interfaces
{
    public interface IBlogsService
    {
        Task<Blog?> AddBlog(InputBlogDto dto);
        Task<bool> UpdateBlog(int id, InputBlogDto dto);
    }
}
