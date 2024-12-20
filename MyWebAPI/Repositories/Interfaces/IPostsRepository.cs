using MyWebAPI.Dto;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IPostsRepository
    {
        Task<PaginatorDto<Post>> GetPosts(GetPostsQueryDto dto);
        Task<PaginatorDto<Post>?> GetPostsForBlog(int blogId, GetPostsQueryDto dto);
        Task<Post?> GetPost(int id);
        Task<List<Post>?> AddPosts(List<Post> posts);
        Task<Post?> AddPost(Post post);
        Task<bool> DeletePost(int id);
        Task<bool> DeleteAllPosts();
        Task<bool> SaveChanges();
    }
}
