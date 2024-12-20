using MyWebAPI.Dto.Video;
using MyWebAPI.Models.Video;
using Assert = Xunit.Assert;

namespace MyWebApiTests.Unit
{
    public class VideoUnitTests
    {
        [Fact]
        public void CreateVideo()
        {
            // Arrange

            var dto = new CreateVideoDto()
            {
                Title = "Title",
                Author = "Author",
                AvaliableResolutions = [VideoResolutions.P144, VideoResolutions.P360]
            };

            // Act

            var video = new Video(dto);

            // Assert

            Assert.Equal(dto.Title, video.Title);
            Assert.Equal(dto.Author, video.Author);
            Assert.Equal(dto.AvaliableResolutions, video.AvaliableResolutions);
        }

        [Fact]
        public void UpdateVideo()
        {
            // Arrange

            var dto = new UpdateVideoDto()
            {
                Title = "Title",
                Author = "Author",
                AvaliableResolutions = [VideoResolutions.P144, VideoResolutions.P360],
                CanBeDownload = true,
                MinAgeRestriction = 12,
                PublicationDate = DateTime.Now,
            };

            // Act

            var video = new Video();
            video.Update(dto);

            // Assert

            Assert.Equal(dto.Title, video.Title);
            Assert.Equal(dto.Author, video.Author);
            Assert.Equal(dto.AvaliableResolutions, video.AvaliableResolutions);
            Assert.Equal(dto.CanBeDownload, video.CanBeDownload);
            Assert.Equal(dto.MinAgeRestriction, video.MinAgeRestriction);
            Assert.Equal(dto.PublicationDate, video.PublicationDate);
        }
    }
}
