using Microsoft.EntityFrameworkCore;
using MyWebAPI.Data;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Extensions;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using MyWebAPI.Repositories.Interfaces;

namespace MyWebAPI.Repositories
{
    public class BlogsRepository : IBlogsRepository
    {
        private readonly MainDbContext _context;

        public BlogsRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<Blog?> AddBlog(Blog blog)
        {
            try
            {
                await _context.Blogs.AddAsync(blog);
                await _context.SaveChangesAsync();
                return blog;
            }
            catch (Exception ex) { return null; }
        }

        public async Task<bool> DeleteBlog(int id)
        {
            try
            {
                var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
                if (blog is null) return false;

                _context.Remove(blog);

                var posts = await _context.Posts.Where(p => p.BlogId == blog.Id).ToListAsync();
                _context.RemoveRange(posts);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<bool> DeleteAllBlogs()
        {
            try
            {
                var blogs = await _context.Blogs.ToListAsync();
                _context.Blogs.RemoveRange(blogs);

                var posts = await _context.Posts.ToListAsync();
                _context.RemoveRange(posts);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }

        public async Task<Blog?> GetBlog(int id)
        {
            return await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PaginatorDto<Blog>> GetBlogs(GetBlogsQueryDto dto)
        {
            var blogs = await _context.Blogs
                .Where(x => x.Name.Contains(dto.SearchNameTerm))
                .OrderByColumn(dto.SortBy, dto.SortDirection)                
                .ToListAsync();

            return new PaginatorDto<Blog>(blogs, dto);
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

        public async Task<List<Blog>?> AddBlogs(List<Blog> blogs)
        {
            try
            {
                await _context.AddRangeAsync(blogs);
                await _context.SaveChangesAsync();
                return blogs;
            }
            catch (Exception ex) { return null; }
        }
    }
}
