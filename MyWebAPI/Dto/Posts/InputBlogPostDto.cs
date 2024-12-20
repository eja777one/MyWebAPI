using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.Posts
{
    public class InputBlogPostDto
    {
        [Required]
        [MaxLength(30)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string ShortDescription { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = null!;

        public InputBlogPostDto() { }
        public InputBlogPostDto(string title, string shortDescription, string content)
        {
            Title = title;
            ShortDescription = shortDescription;
            Content = content;
        }
    }
}
