using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
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

        public async Task<List<Post>> GetPosts()
        {
            return await _context.Posts.ToListAsync() ?? new();
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
    }
}
