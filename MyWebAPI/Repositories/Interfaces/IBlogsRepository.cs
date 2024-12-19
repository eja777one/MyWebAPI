using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IBlogsRepository
    {
        Task<List<Blog>> GetBlogs();
        Task<Blog?> GetBlog(int id);
        Task<Blog?> AddBlog(Blog blog);
        Task<bool> DeleteBlog(int id);
        Task<bool> DeleteAllBlogs();
        Task<bool> SaveChanges();
    }
}
