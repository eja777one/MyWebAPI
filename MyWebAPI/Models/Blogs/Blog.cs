using MyWebAPI.Dto.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebAPI.Models.Blogs
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;

        public Blog()
        {

        }

        public Blog(InputBlogDto dto)
        {
            Update(dto);
        }

        public void Update(InputBlogDto dto)
        {
            Name = dto.Name;
            Description = dto.Description;
            WebsiteUrl = dto.WebsiteUrl;
        }
    }
}
