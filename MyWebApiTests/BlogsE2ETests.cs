using System.Net.Http.Json;
using System.Net;
using MyWebAPI.Models.Blogs;
using Microsoft.AspNetCore.Http;
using MyWebAPI.Models.Posts;

namespace MyWebApiTests
{
    [Collection("Blogs")]
    public class BlogsE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        private string _postsAbsUrl = "";
        private readonly string _postsRelUrl = "/posts/";

        private string _blogsAbsUrl = "";
        private readonly string _blogsRelUrl = "/blogs/";

        public BlogsE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost");
            _postsAbsUrl = $"{_client.BaseAddress}{_postsRelUrl.Substring(1)}";
            _blogsAbsUrl = $"{_client.BaseAddress}{_blogsRelUrl.Substring(1)}";
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

        [Fact]
        public async Task Test01_GetBlogs()
        {
            // Arrange

            // Act
            var httpResponse = await _client.GetAsync(_blogsRelUrl);
            var blogs = await httpResponse.Content.ReadFromJsonAsync<Blog[]>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse?.StatusCode);
            Assert.Equal(0, blogs?.Length);
        }

        [Fact]
        public async Task Test02_CreateBlog()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var badInputBlogDto = TestData.GetIncorrectInputBlogDto();

            var requestMessage = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, badInputBlogDto);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            // Act
            var httpResponse1 = await _client.PostAsJsonAsync(_blogsRelUrl, inputBlogDto);

            var httpResponse2 = await _client.SendAsync(requestMessage);

            var httpResponse3 = await _client.SendAsync(requestMessage2);
            var blog = await httpResponse3.Content.ReadFromJsonAsync<Blog>();

            var httpResponse4 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}");
            var blogDb = await httpResponse4.Content.ReadFromJsonAsync<Blog>();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse1?.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.Created, httpResponse3?.StatusCode);
            Assert.Equal(inputBlogDto.Name, blog?.Name);
            Assert.Equal(inputBlogDto.Description, blog?.Description);
            Assert.Equal(inputBlogDto.WebsiteUrl, blog?.WebsiteUrl);

            Assert.Equal(HttpStatusCode.OK, httpResponse4?.StatusCode);
            Assert.Equal(inputBlogDto.Name, blogDb?.Name);
            Assert.Equal(inputBlogDto.Description, blogDb?.Description);
            Assert.Equal(inputBlogDto.WebsiteUrl, blogDb?.WebsiteUrl);
        }

        [Fact]
        public async Task Test03_UpdateBlog()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var blog = await httpResponse1.Content.ReadFromJsonAsync<Blog>();

            var updateBlogDto = TestData.GetCorrectInputBlogDto();
            var badUpdateBlogDto = TestData.GetIncorrectInputBlogDto();

            var requestMessage = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_blogsAbsUrl}{blog?.Id}", badUpdateBlogDto);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_blogsAbsUrl}{blog?.Id}", updateBlogDto);
            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_blogsAbsUrl}/999", updateBlogDto);

            // Act
            var httpResponse2 = await _client.SendAsync(requestMessage3);

            var httpResponse3 = await _client.PutAsJsonAsync(requestMessage2.RequestUri, requestMessage2.Content);

            var httpResponse4 = await _client.SendAsync(requestMessage);

            var httpResponse5 = await _client.SendAsync(requestMessage2);

            var httpResponse6 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}");
            var blogDb = await httpResponse6.Content.ReadFromJsonAsync<Blog>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse3?.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse4?.StatusCode);

            Assert.Equal(HttpStatusCode.NoContent, httpResponse5?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse6?.StatusCode);
            Assert.Equal(updateBlogDto.Name, blogDb?.Name);
            Assert.Equal(updateBlogDto.Description, blogDb?.Description);
            Assert.Equal(updateBlogDto.WebsiteUrl, blogDb?.WebsiteUrl);
        }

        [Fact]
        public async Task Test04_DeleteBlog()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var blog = await httpResponse1.Content.ReadFromJsonAsync<Blog>();

            var inputPostDto = TestData.GetCorrectInputPostDto(blog?.Id ?? 999);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, _postsAbsUrl, inputPostDto);

            var httpResponse2 = await _client.SendAsync(requestMessage2);
            var post = await httpResponse2.Content.ReadFromJsonAsync<Post>();

            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_blogsRelUrl}999", null);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_blogsRelUrl}{blog?.Id}", null);

            // Act

            var httpResponse3 = await _client.SendAsync(requestMessage3);

            var httpResponse4 = await _client.DeleteAsync($"{_blogsRelUrl}{blog?.Id}");

            var httpResponse5 = await _client.SendAsync(requestMessage4);

            var httpResponse6 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}");

            var httpResponse7 = await _client.GetAsync($"{_postsRelUrl}{post?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.Created, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse4?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse5?.StatusCode);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse6?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse7?.StatusCode);
        }
    }
}
