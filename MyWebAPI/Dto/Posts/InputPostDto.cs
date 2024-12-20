using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.Posts
{
    public class InputPostDto : InputBlogPostDto
    {
        [Required]
        public int? BlogId { get; set; }

        public InputPostDto() { }

        public InputPostDto(string title, string shortDescription, string content, int blogId)
            : base(title, shortDescription, content)
        {
            BlogId = blogId;
        }
    }
}
