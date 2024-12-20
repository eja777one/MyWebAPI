using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Services.Interfaces
{
    public interface IPostsService
    {
        Task<Post?> AddPost(InputPostDto dto);
        Task<Post?> AddPostForBlog(int blogId, InputBlogPostDto dto);
        Task<bool> UpdatePost(int id, InputPostDto dto);
    }
}
