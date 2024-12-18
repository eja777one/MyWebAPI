using System.ComponentModel.DataAnnotations;
using MyWebAPI.Models.Video;

namespace MyWebAPI.Dto.Video
{
    public class CreateVideoDto
    {
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string Author { get; set; }

        [Required]
        [MinLength(1)]
        public List<VideoResolutions> AvaliableResolutions { get; set; } = new();

        public CreateVideoDto() { }
        public CreateVideoDto(string title, string author, List<VideoResolutions> resolutions)
        {
            Title = title;
            Author = author;
            AvaliableResolutions = resolutions;
        }
    }
}
