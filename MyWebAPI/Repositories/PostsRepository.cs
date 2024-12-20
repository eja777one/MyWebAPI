using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyWebAPI.Data;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Extensions;
using MyWebAPI.Models.Blogs;
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
            try
            {
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                return post;
            }
            catch (Exception ex) { return null; }
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
            try
            {
                var post = await _context.Posts.FirstOrDefaultAsync(b => b.Id == id);
                if (post is null) return false;
                _context.Remove(post);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteAllPosts()
        {
            try
            {
                var posts = await _context.Posts.ToListAsync();
                _context.Posts.RemoveRange(posts);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
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
            try
            {
                await _context.AddRangeAsync(posts);
                await _context.SaveChangesAsync();
                return posts;
            }
            catch (Exception ex) { return null; }

        }
    }
}
