using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.Blogs
{
    public class InputBlogDto
    {
        [Required]
        [MaxLength(15)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required]
        [RegularExpression("^https://([a-zA-Z0-9_-]+\\.)+[a-zA-Z0-9_-]+(\\/[a-zA-Z0-9_-]+)*\\/?$")]
        public string WebsiteUrl { get; set; } = null!;

        public InputBlogDto()
        {

        }

        public InputBlogDto(string name, string description, string websiteUrl)
        {
            Name = name;
            Description = description;
            WebsiteUrl = websiteUrl;
        }

        public InputBlogDto(string description, string websiteUrl)
        {
            Description = description;
            WebsiteUrl = websiteUrl;
        }

    }
}
