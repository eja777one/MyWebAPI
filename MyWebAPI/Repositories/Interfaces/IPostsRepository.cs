using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IPostsRepository
    {
        Task<List<Post>> GetPosts();
        Task<Post?> GetPost(int id);
        Task<Post?> AddPost(Post post);
        Task<bool> DeletePost(int id);
        Task<bool> DeleteAllPosts();
        Task<bool> SaveChanges();
    }
}
