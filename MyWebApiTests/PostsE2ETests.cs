using System.Net.Http.Json;
using System.Net;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebApiTests
{
    [Collection("Posts")]
    public class PostsE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        private string _postsAbsUrl = "";
        private readonly string _postsRelUrl = "/posts/";

        private string _blogsAbsUrl = "";
        private readonly string _blogsRelUrl = "/blogs/";

        public PostsE2ETests(CustomWebApplicationFactory factory)
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
        public async Task Test01_GetPosts()
        {
            // Arrange

            // Act
            var httpResponse = await _client.GetAsync(_postsRelUrl);
            var posts = await httpResponse.Content.ReadFromJsonAsync<Post[]>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse?.StatusCode);
            Assert.Equal(0, posts?.Length);
        }

        [Fact]
        public async Task Test02_CreatePost()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var blog = await httpResponse1.Content.ReadFromJsonAsync<Blog>();

            var inputPostDto = TestData.GetCorrectInputPostDto(blog?.Id ?? 999);
            var badInputPostDto = TestData.GetIncorrectInputPostDto();

            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Post, _postsAbsUrl, badInputPostDto);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Post, _postsAbsUrl, inputPostDto);

            // Act
            var httpResponse2 = await _client.PostAsJsonAsync(_postsRelUrl, inputBlogDto);

            var httpResponse3 = await _client.SendAsync(requestMessage3);

            var httpResponse4 = await _client.SendAsync(requestMessage4);
            var post = await httpResponse4.Content.ReadFromJsonAsync<Post>();

            var httpResponse5 = await _client.GetAsync($"{_postsRelUrl}{post?.Id}");
            var postDb = await httpResponse5.Content.ReadFromJsonAsync<Post>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse3?.StatusCode);

            Assert.Equal(HttpStatusCode.Created, httpResponse4?.StatusCode);
            Assert.Equal(inputPostDto.Title, post?.Title);
            Assert.Equal(inputPostDto.Content, post?.Content);
            Assert.Equal(inputPostDto.ShortDescription, post?.ShortDescription);
            Assert.Equal(inputPostDto.BlogId, post?.BlogId);

            Assert.Equal(HttpStatusCode.OK, httpResponse5?.StatusCode);
            Assert.Equal(inputPostDto.Title, postDb?.Title);
            Assert.Equal(inputPostDto.Content, postDb?.Content);
            Assert.Equal(inputPostDto.ShortDescription, postDb?.ShortDescription);
            Assert.Equal(inputPostDto.BlogId, postDb?.BlogId);
        }

        [Fact]
        public async Task Test03_UpdatePost()
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

            var updatePostDto = TestData.GetCorrectInputPostDto(blog?.Id ?? 999);
            var badUpdatePostDto = TestData.GetIncorrectInputPostDto();

            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_postsAbsUrl}999", updatePostDto);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_postsAbsUrl}{post?.Id}", updatePostDto);
            var requestMessage5 = TestData.GetAuthorizedMessage(HttpMethod.Put, $"{_postsAbsUrl}{post?.Id}", badUpdatePostDto);

            // Act
            var httpResponse3 = await _client.SendAsync(requestMessage3);

            var httpResponse4 = await _client.PutAsJsonAsync(requestMessage4.RequestUri, requestMessage4.Content);

            var httpResponse5 = await _client.SendAsync(requestMessage5);

            var httpResponse6 = await _client.SendAsync(requestMessage4);

            var httpResponse7 = await _client.GetAsync($"{_postsRelUrl}{post?.Id}");
            var postDb = await httpResponse7.Content.ReadFromJsonAsync<Post>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.Created, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse4?.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse5?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse6?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse7?.StatusCode);
            Assert.Equal(updatePostDto.Title, postDb?.Title);
            Assert.Equal(updatePostDto.Content, postDb?.Content);
            Assert.Equal(updatePostDto.ShortDescription, postDb?.ShortDescription);
            Assert.Equal(updatePostDto.BlogId, postDb?.BlogId);
        }

        [Fact]
        public async Task Test04_DeletePost()
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

            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_postsRelUrl}999", null);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_postsRelUrl}{post?.Id}", null);

            // Act

            var httpResponse3 = await _client.SendAsync(requestMessage3);

            var httpResponse4 = await _client.DeleteAsync($"{_postsRelUrl}{post?.Id}");

            var httpResponse5 = await _client.SendAsync(requestMessage4);

            var httpResponse6 = await _client.GetAsync($"{_postsRelUrl}{post?.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.Created, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse4?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse5?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse6?.StatusCode);
        }

    }
}
