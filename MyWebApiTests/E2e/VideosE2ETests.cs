using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Video;
using MyWebAPI.Models.Video;
using Newtonsoft.Json;

namespace MyWebApiTests.E2e
{
    [Collection("Videos")]
    public class VideosE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;

        public VideosE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

        [Fact]
        public async Task Test01_GetVideos()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://localhost");

            // Act
            var httpResponse = await client.GetAsync("/videos");
            var videos = await httpResponse.Content.ReadFromJsonAsync<Video[]>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse?.StatusCode);
            Assert.Equal(0, videos?.Length);
        }

        [Fact]
        public async Task Test02_CreateVideo()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://localhost");

            var createVideoDto = new CreateVideoDto("Title", "Author", [VideoResolutions.P144]);
            var createVideoBadDto = new CreateVideoDto() { Title = "Title" };

            var exceptedErr = new ApiErrorResultDto();
            exceptedErr.ErrorsMessages.Add(new("Incorrect author", "Author"));

            // Act
            var httpResponse = await client.PostAsJsonAsync("/videos", createVideoBadDto);
            var errObject = await httpResponse.Content.ReadFromJsonAsync<ApiErrorResultDto>();

            var httpResponse2 = await client.PostAsJsonAsync("/videos", createVideoDto);
            var video = await httpResponse2.Content.ReadFromJsonAsync<Video>();

            var httpResponse3 = await client.GetAsync($"/videos/{video?.Id}");
            var dbVideo = await httpResponse3.Content.ReadFromJsonAsync<Video>();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse?.StatusCode); // excepted | actual
            //Assert.Equal(exceptedErr.ErrorsMessages, errObject?.ErrorsMessages);

            Assert.Equal(HttpStatusCode.Created, httpResponse2?.StatusCode);

            Assert.Equal(video?.Title, createVideoDto.Title);
            Assert.Equal(video?.Author, createVideoDto.Author);
            Assert.Equal(video?.AvaliableResolutions, createVideoDto.AvaliableResolutions);

            Assert.Equal(HttpStatusCode.OK, httpResponse3?.StatusCode);

            Assert.Equal(dbVideo?.Title, createVideoDto.Title);
            Assert.Equal(dbVideo?.Author, createVideoDto.Author);
            Assert.Equal(dbVideo?.AvaliableResolutions, createVideoDto.AvaliableResolutions);
        }

        [Fact]
        public async Task Test03_UpdateVideo()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://localhost");

            var createVideoDto = new CreateVideoDto("Title", "Author", [VideoResolutions.P144]);

            var updateVideoDto = new UpdateVideoDto()
            {
                Title = "NewTitle",
                Author = "NewAuthor",
                CanBeDownload = true,
                MinAgeRestriction = 6,
                PublicationDate = DateTime.Now,
                AvaliableResolutions = new() { VideoResolutions.P360, VideoResolutions.P480 }
            };

            var updateVideoDtoBad = new UpdateVideoDto()
            {
                Title = null,
                Author = null,
                CanBeDownload = true,
                MinAgeRestriction = 20,
                PublicationDate = null,
                AvaliableResolutions = new()
            };

            var exceptedErr = new ApiErrorResultDto();
            exceptedErr.ErrorsMessages.Add(new("Incorrect author", "Author"));

            var content = new StringContent(JsonConvert.SerializeObject(updateVideoDto), Encoding.UTF8, "application/json");
            var badContent = new StringContent(JsonConvert.SerializeObject(updateVideoDtoBad), Encoding.UTF8, "application/json");

            // Act
            var httpResponse = await client.PostAsJsonAsync("/videos", createVideoDto);
            var video = await httpResponse.Content.ReadFromJsonAsync<Video>();

            var httpResponse2 = await client.PutAsync($"/videos/999", content);

            var httpResponse3 = await client.PutAsync($"/videos/{video?.Id}", badContent);
            var errObject = await httpResponse3.Content.ReadFromJsonAsync<ApiErrorResultDto>();

            var httpResponse4 = await client.PutAsync($"/videos/{video?.Id}", content);

            var httpResponse5 = await client.GetAsync($"/videos/{video?.Id}");
            var dbVideo = await httpResponse5.Content.ReadFromJsonAsync<Video>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse?.StatusCode);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse3?.StatusCode);
            //Assert.Equal(exceptedErr.ErrorsMessages, errObject?.ErrorsMessages);

            Assert.Equal(HttpStatusCode.NoContent, httpResponse4?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse5?.StatusCode);

            Assert.Equal(updateVideoDto.Title, dbVideo?.Title);
            Assert.Equal(updateVideoDto.Author, dbVideo?.Author);
            Assert.Equal(updateVideoDto.CanBeDownload, dbVideo?.CanBeDownload);
            Assert.Equal(updateVideoDto.MinAgeRestriction, dbVideo?.MinAgeRestriction);
            //Assert.Equal(updateVideoDto.PublicationDate, dbVideo?.PublicationDate);
            Assert.Equal(updateVideoDto.AvaliableResolutions, dbVideo?.AvaliableResolutions);
        }

        [Fact]
        public async Task Test04_DeleteVideo()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("https://localhost");

            var createVideoDto = new CreateVideoDto("Title", "Author", [VideoResolutions.P144]);

            // Act
            var httpResponse = await client.PostAsJsonAsync("/videos", createVideoDto);
            var video = await httpResponse.Content.ReadFromJsonAsync<Video>();

            var httpResponse2 = await client.DeleteAsync($"/videos/999");

            var httpResponse3 = await client.DeleteAsync($"/videos/{video?.Id}");

            var httpResponse4 = await client.GetAsync($"/videos/{video?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse4?.StatusCode);
        }
    }

}
