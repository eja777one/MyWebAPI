using System.Net.Http.Json;
using System.Net;
using MyWebAPI.Models.Blogs;
using Microsoft.AspNetCore.Http;
using MyWebAPI.Models.Posts;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Dto.Blogs;

namespace MyWebApiTests.E2e
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

        private string _testingAbsUrl = "";
        private readonly string _testingRelUrl = "/testing/";

        public BlogsE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost");
            _postsAbsUrl = $"{_client.BaseAddress}{_postsRelUrl.Substring(1)}";
            _blogsAbsUrl = $"{_client.BaseAddress}{_blogsRelUrl.Substring(1)}";
            _testingAbsUrl = $"{_client.BaseAddress}{_testingRelUrl.Substring(1)}";
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

        [Fact]
        public async Task Test01_GetBlogs()
        {
            // Arrange
            var new20Blogs = TestData.GetBlogs(20);
            var requestMessage = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_testingAbsUrl}Blogs", new20Blogs);

            var getBlogsQuery1 = new GetBlogsQueryDto("Super", "Name", "asc", 3, 2);
            var getBlogsQuery2 = new GetBlogsQueryDto("Mega", "WebsiteUrl", "desc", 2, 5);

            var firstReqBlogs = new20Blogs
                .Where(x => x.Name.Contains(getBlogsQuery1.SearchNameTerm))
                .OrderBy(x => x.Name)
                .ToList();

            var firstBlogsCount = firstReqBlogs.Count;
            var firstPagesCount = PaginatorDto<Blog>.GetPagesTotalCount(firstReqBlogs.Count, getBlogsQuery1.PageSize);

            firstReqBlogs = firstReqBlogs
               .Skip((getBlogsQuery1.PageNumber - 1) * getBlogsQuery1.PageSize)
               .Take(getBlogsQuery1.PageSize)
               .ToList();

            var secondReqBlogs = new20Blogs
                .Where(x => x.Name.Contains(getBlogsQuery2.SearchNameTerm))
                .OrderByDescending(x => x.WebsiteUrl)
                .ToList();

            var secondBlogsCount = secondReqBlogs.Count;
            var secondPagesCount = PaginatorDto<Blog>.GetPagesTotalCount(secondReqBlogs.Count, getBlogsQuery2.PageSize);

            secondReqBlogs = secondReqBlogs
               .Skip((getBlogsQuery2.PageNumber - 1) * getBlogsQuery2.PageSize)
               .Take(getBlogsQuery2.PageSize)
               .ToList();

            var httpResponse1 = await _client.SendAsync(requestMessage);

            // Act
            var httpResponse2 = await _client.GetAsync($"{_blogsRelUrl.Substring(1)}?pageSize={getBlogsQuery1.PageSize}&pageNumber={getBlogsQuery1.PageNumber}&searchNameTerm={getBlogsQuery1.SearchNameTerm}&sortBy={getBlogsQuery1.SortBy}&sortDirection={getBlogsQuery1.SortDirection}");
            var paginatorBlogs2 = await httpResponse2.Content.ReadFromJsonAsync<PaginatorDto<Blog>>();

            var httpResponse3 = await _client.GetAsync($"{_blogsRelUrl.Substring(1)}?pageSize={getBlogsQuery2.PageSize}&pageNumber={getBlogsQuery2.PageNumber}&searchNameTerm={getBlogsQuery2.SearchNameTerm}&sortBy={getBlogsQuery2.SortBy}");
            var paginatorBlogs3 = await httpResponse3.Content.ReadFromJsonAsync<PaginatorDto<Blog>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse1?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse2?.StatusCode);

            Assert.Equal(firstPagesCount, paginatorBlogs2?.PagesCount);
            Assert.Equal(getBlogsQuery1.PageNumber, paginatorBlogs2?.Page);
            Assert.Equal(getBlogsQuery1.PageSize, paginatorBlogs2?.PageSize);
            Assert.Equal(firstBlogsCount, paginatorBlogs2?.TotalCount);

            Assert.Equal(firstReqBlogs.Count, paginatorBlogs2?.Items.Count);
            Assert.Equal(firstReqBlogs.Last().Name, paginatorBlogs2?.Items.Last().Name);
            Assert.Equal(firstReqBlogs.Last().Description, paginatorBlogs2?.Items.Last().Description);
            Assert.Equal(firstReqBlogs.Last().WebsiteUrl, paginatorBlogs2?.Items.Last().WebsiteUrl);

            Assert.Equal(HttpStatusCode.OK, httpResponse3?.StatusCode);

            Assert.Equal(secondPagesCount, paginatorBlogs3?.PagesCount);
            Assert.Equal(getBlogsQuery2.PageNumber, paginatorBlogs3?.Page);
            Assert.Equal(getBlogsQuery2.PageSize, paginatorBlogs3?.PageSize);
            Assert.Equal(secondBlogsCount, paginatorBlogs3?.TotalCount);

            Assert.Equal(secondReqBlogs.Count, paginatorBlogs3?.Items.Count);
            Assert.Equal(secondReqBlogs.Last().Name, paginatorBlogs3?.Items.Last().Name);
            Assert.Equal(secondReqBlogs.Last().Description, paginatorBlogs3?.Items.Last().Description);
            Assert.Equal(secondReqBlogs.Last().WebsiteUrl, paginatorBlogs3?.Items.Last().WebsiteUrl);
        }

        [Fact]
        public async Task Test02_CreateBlog()
        {
            // Arrange
            var createdTime = DateTime.UtcNow;

            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var badInputBlogDto = TestData.GetIncorrectInputBlogDto();

            var requestMessage = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, badInputBlogDto);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            // Act
            var httpResponse1 = await _client.PostAsJsonAsync(_blogsRelUrl, inputBlogDto);

            var httpResponse2 = await _client.SendAsync(requestMessage);

            var httpResponse3 = await _client.SendAsync(requestMessage2);
            var blog = await httpResponse3.Content.ReadFromJsonAsync<Blog>();

            var timeDiff = blog?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            var httpResponse4 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}");
            var blogDb = await httpResponse4.Content.ReadFromJsonAsync<Blog>();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse1?.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.Created, httpResponse3?.StatusCode);
            Assert.Equal(inputBlogDto.Name, blog?.Name);
            Assert.Equal(inputBlogDto.Description, blog?.Description);
            Assert.Equal(inputBlogDto.WebsiteUrl, blog?.WebsiteUrl);
            Assert.True(isLessThan2Min);

            Assert.Equal(HttpStatusCode.OK, httpResponse4?.StatusCode);
            Assert.Equal(inputBlogDto.Name, blogDb?.Name);
            Assert.Equal(inputBlogDto.Description, blogDb?.Description);
            Assert.Equal(inputBlogDto.WebsiteUrl, blogDb?.WebsiteUrl);
        }

        [Fact]
        public async Task Test03_GetPostsForBlog()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var blog = await httpResponse1.Content.ReadFromJsonAsync<Blog>();

            var new20Posts = TestData.GetPosts(20, blog!.Id, blog!.Name);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_testingAbsUrl}Posts", new20Posts);
            var httpResponse2 = await _client.SendAsync(requestMessage2);

            var getPostsQuery1 = new GetPostsQueryDto("Title", "asc", 3, 2);
            var getPostsQuery2 = new GetPostsQueryDto("ShortDescription", "desc", 2, 5);

            var firstReqPosts = new20Posts.OrderBy(x => x.Title).ToList();

            var firstPostsCount = firstReqPosts.Count;
            var firstPagesCount = PaginatorDto<Post>.GetPagesTotalCount(firstPostsCount, getPostsQuery1.PageSize);

            firstReqPosts = firstReqPosts
               .Skip((getPostsQuery1.PageNumber - 1) * getPostsQuery1.PageSize)
               .Take(getPostsQuery1.PageSize)
               .ToList();

            var secondReqPosts = new20Posts.OrderByDescending(x => x.ShortDescription).ToList();

            var secondPostsCount = secondReqPosts.Count;
            var secondPagesCount = PaginatorDto<Post>.GetPagesTotalCount(secondPostsCount, getPostsQuery2.PageSize);

            secondReqPosts = secondReqPosts
               .Skip((getPostsQuery2.PageNumber - 1) * getPostsQuery2.PageSize)
               .Take(getPostsQuery2.PageSize)
               .ToList();

            var postUrl = _postsRelUrl.Substring(0, _postsRelUrl.Length - 1);

            // Act
            var httpResponse3 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}{postUrl}?pageSize={getPostsQuery1.PageSize}&pageNumber={getPostsQuery1.PageNumber}&sortBy={getPostsQuery1.SortBy}&sortDirection={getPostsQuery1.SortDirection}");
            var paginatorPosts3 = await httpResponse3.Content.ReadFromJsonAsync<PaginatorDto<Post>>();

            var httpResponse4 = await _client.GetAsync($"{_blogsRelUrl}{blog?.Id}{postUrl}?pageSize={getPostsQuery2.PageSize}&pageNumber={getPostsQuery2.PageNumber}&sortBy={getPostsQuery2.SortBy}");
            var paginatorPosts4 = await httpResponse4.Content.ReadFromJsonAsync<PaginatorDto<Post>>();

            var httpResponse5 = await _client.GetAsync($"{_blogsRelUrl}999{postUrl}?pageSize={getPostsQuery2.PageSize}&pageNumber={getPostsQuery2.PageNumber}&sortBy={getPostsQuery2.SortBy}");

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.OK, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse3?.StatusCode); // err
            Assert.Equal(firstPagesCount, paginatorPosts3?.PagesCount);
            Assert.Equal(getPostsQuery1.PageNumber, paginatorPosts3?.Page);
            Assert.Equal(getPostsQuery1.PageSize, paginatorPosts3?.PageSize);
            Assert.Equal(firstPostsCount, paginatorPosts3?.TotalCount);

            Assert.Equal(firstReqPosts.Count, paginatorPosts3?.Items.Count);
            Assert.Equal(firstReqPosts.Last().Title, paginatorPosts3?.Items.Last().Title);
            Assert.Equal(firstReqPosts.Last().Content, paginatorPosts3?.Items.Last().Content);
            Assert.Equal(firstReqPosts.Last().ShortDescription, paginatorPosts3?.Items.Last().ShortDescription);
            Assert.Equal(firstReqPosts.Last().BlogId, paginatorPosts3?.Items.Last().BlogId);

            Assert.Equal(HttpStatusCode.OK, httpResponse4?.StatusCode);
            Assert.Equal(secondPagesCount, paginatorPosts4?.PagesCount);
            Assert.Equal(getPostsQuery2.PageNumber, paginatorPosts4?.Page);
            Assert.Equal(getPostsQuery2.PageSize, paginatorPosts4?.PageSize);
            Assert.Equal(secondPostsCount, paginatorPosts4?.TotalCount);

            Assert.Equal(secondReqPosts.Last().Title, paginatorPosts4?.Items.Last().Title);
            Assert.Equal(secondReqPosts.Last().Content, paginatorPosts4?.Items.Last().Content);
            Assert.Equal(secondReqPosts.Last().ShortDescription, paginatorPosts4?.Items.Last().ShortDescription);
            Assert.Equal(secondReqPosts.Last().BlogId, paginatorPosts4?.Items.Last().BlogId);

            Assert.Equal(HttpStatusCode.NotFound, httpResponse5?.StatusCode);
        }

        [Fact]
        public async Task Test04_CreatePostsForBlog()
        {
            // Arrange
            var createdTime = DateTime.UtcNow;

            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _blogsAbsUrl, inputBlogDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var blog = await httpResponse1.Content.ReadFromJsonAsync<Blog>();

            var inputPostDto = TestData.GetCorrectInputBlogPostDto();
            var badInputPostDto = TestData.GetIncorrectInputBlogPostDto();

            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_blogsAbsUrl}999{_postsRelUrl}", inputPostDto);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_blogsAbsUrl}{blog?.Id}{_postsRelUrl}", badInputPostDto);
            var requestMessage5 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_blogsAbsUrl}{blog?.Id}{_postsRelUrl}", inputPostDto);

            // Act
            var httpResponse2 = await _client.SendAsync(requestMessage2);

            var httpResponse3 = await _client.PostAsJsonAsync($"{_blogsRelUrl}999{_postsRelUrl}", inputBlogDto);

            var httpResponse4 = await _client.SendAsync(requestMessage4);

            var httpResponse5 = await _client.SendAsync(requestMessage5);
            var post = await httpResponse5.Content.ReadFromJsonAsync<Post>();

            var timeDiff = post?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            var httpResponse6 = await _client.GetAsync($"{_postsRelUrl}{post?.Id}");
            var postDb = await httpResponse6.Content.ReadFromJsonAsync<Post>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse4?.StatusCode);

            Assert.Equal(HttpStatusCode.Created, httpResponse5?.StatusCode);
            Assert.Equal(inputPostDto.Title, post?.Title);
            Assert.Equal(inputPostDto.Content, post?.Content);
            Assert.Equal(inputPostDto.ShortDescription, post?.ShortDescription);
            Assert.Equal(blog?.Id, post?.BlogId);
            Assert.True(isLessThan2Min);

            Assert.Equal(HttpStatusCode.OK, httpResponse6?.StatusCode);
            Assert.Equal(inputPostDto.Title, postDb?.Title);
            Assert.Equal(inputPostDto.Content, postDb?.Content);
            Assert.Equal(inputPostDto.ShortDescription, postDb?.ShortDescription);
            Assert.Equal(blog?.Id, postDb?.BlogId);
        }

        [Fact]
        public async Task Test05_UpdateBlog()
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
        public async Task Test06_DeleteBlog()
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
