using System.ComponentModel.DataAnnotations;
using MyWebAPI.Models.Video;

namespace MyWebAPI.Dto.Video
{
    public class UpdateVideoDto
    {
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string Author { get; set; }

        [Required]
        [MinLength(1)]
        public List<VideoResolutions> AvaliableResolutions { get; set; }

        public bool CanBeDownload { get; set; } = false;

        [Range(1, 18)]
        public int? MinAgeRestriction { get; set; }

        public DateTime? PublicationDate { get; set; }
    }
}
