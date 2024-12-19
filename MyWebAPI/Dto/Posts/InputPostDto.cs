using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.Posts
{
    public class InputPostDto
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

        [Required]
        public int? BlogId { get; set; }

        public InputPostDto()
        {

        }

        public InputPostDto(string title, string shortDescription, string content, int blogId)
        {
            Title = title;
            ShortDescription = shortDescription;
            Content = content;
            BlogId = blogId;
        }

        public InputPostDto(string title, string shortDescription, string content)
        {
            Title = title;
            ShortDescription = shortDescription;
            Content = content;
        }
    }
}
