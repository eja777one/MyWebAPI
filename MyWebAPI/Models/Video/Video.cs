using MyWebAPI.Dto.Video;

namespace MyWebAPI.Models.Video
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool CanBeDownload { get; set; }
        public int MinAgeRestriction { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<VideoResolutions>? AvaliableResolutions { get; set; }

        public Video()
        {

        }

        public Video(CreateVideoDto dto)
        {
            Title = dto.Title;
            Author = dto.Author;
            AvaliableResolutions = dto.AvaliableResolutions;

            CreatedAt = DateTime.UtcNow;
        }

        public void Update(UpdateVideoDto dto)
        {
            Title = dto.Title;
            Author = dto.Author;
            AvaliableResolutions = dto.AvaliableResolutions;
            CanBeDownload = dto.CanBeDownload;
            MinAgeRestriction = dto.MinAgeRestriction ?? MinAgeRestriction;
            PublicationDate = dto.PublicationDate ?? PublicationDate;
        }
    }

    public enum VideoResolutions
    {
        P144,
        P240,
        P360,
        P480,
        P720,
        P1080,
        P1440,
        P2160
    }
}
