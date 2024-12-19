using MyWebAPI.Dto.Blogs;

namespace MyWebAPI.Models.Blogs
{
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string WebsiteUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsMembership { get; set; }

        public Blog()
        {

        }

        public Blog(InputBlogDto dto)
        {
            Update(dto);
            CreatedAt = DateTime.UtcNow;
        }

        public void Update(InputBlogDto dto)
        {
            Name = dto.Name;
            Description = dto.Description;
            WebsiteUrl = dto.WebsiteUrl;
        }
    }
}
