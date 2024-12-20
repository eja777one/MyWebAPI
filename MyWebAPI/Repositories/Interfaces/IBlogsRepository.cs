using MyWebAPI.Dto;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IBlogsRepository
    {
        Task<PaginatorDto<Blog>> GetBlogs(GetBlogsQueryDto dto);
        Task<Blog?> GetBlog(int id);
        Task<Blog?> AddBlog(Blog blog);
        Task<List<Blog>?> AddBlogs(List<Blog> blogs);
        Task<bool> DeleteBlog(int id);
        Task<bool> DeleteAllBlogs();
        Task<bool> SaveChanges();
    }
}
