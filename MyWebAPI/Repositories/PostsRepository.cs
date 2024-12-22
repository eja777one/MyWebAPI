using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Extensions;
using MyWebAPI.Models.Posts;
using MyWebAPI.Repositories.Interfaces;

namespace MyWebAPI.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly MainDbContext _context;

        public PostsRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<Post?> AddPost(Post post)
        {
            await _context.Posts.AddAsync(post);
            return await SaveChanges() ? post : null;
        }

        public async Task<Post?> GetPost(int id)
        {
            return await _context.Posts.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeletePost(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(b => b.Id == id);
            if (post is null) return false;
            _context.Remove(post);

            return await SaveChanges();
        }

        public async Task<bool> DeleteAllPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            _context.Posts.RemoveRange(posts);

            return await SaveChanges();
        }

        public async Task<PaginatorDto<Post>> GetPosts(GetPostsQueryDto dto)
        {
            var posts = await _context.Posts
                .OrderByColumn(dto.SortBy, dto.SortDirection)
                .ToListAsync();

            return new PaginatorDto<Post>(posts, dto);
        }

        public async Task<PaginatorDto<Post>?> GetPostsForBlog(int blogId, GetPostsQueryDto dto)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == blogId);
            if (blog is null) return null;

            var posts = await _context.Posts
                .Where(x => x.BlogId == blogId)
                .OrderByColumn(dto.SortBy, dto.SortDirection)
                .ToListAsync();

            return new PaginatorDto<Post>(posts, dto);
        }

        public async Task<List<Post>?> AddPosts(List<Post> posts)
        {
            await _context.AddRangeAsync(posts);
            return await SaveChanges() ? posts : null;
        }
    }
}
