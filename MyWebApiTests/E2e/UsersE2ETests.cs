using System.Net;
using System.Net.Http.Json;
using MyWebAPI.Dto;
using MyWebAPI.Dto.User;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using MyWebAPI.Models.User;
using MyWebAPI.Repositories;

namespace MyWebApiTests.E2e
{
    [Collection("Users")]
    public class UsersE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        private readonly string _authRelUrl = "/auth/login";

        private string _usersAbsUrl = "";
        private readonly string _usersRelUrl = "/users/";

        private string _testingAbsUrl = "";
        private readonly string _testingRelUrl = "/testing/";

        private PasswordService? _passService = null;

        public UsersE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost");
            _usersAbsUrl = $"{_client.BaseAddress}{_usersRelUrl.Substring(1)}";
            _testingAbsUrl = $"{_client.BaseAddress}{_testingRelUrl.Substring(1)}";

            _passService = _factory.Services.GetService(typeof(PasswordService)) as PasswordService;
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

        [Fact]
        public async Task Test01_GetUsers()
        {
            // Arrange
            var new20Users = TestData.GetUsers(20, _passService);
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_testingAbsUrl}Users", new20Users);

            var getUsersQuery1 = new GetUsersQueryDto("Login", "asc", 3, 2, "Super", "Mega");
            var getUsersQuery2 = new GetUsersQueryDto("Email", "desc", 2, 5, "null", "Super");

            var firstReqUsers = new20Users
                .Where(x => x.Login.Contains(getUsersQuery1.SearchLoginTerm))
                .Where(x => x.Email.Contains(getUsersQuery1.SearchEmailTerm))
                .OrderBy(x => x.Login)
                .ToList();

            var firstUsersCount = firstReqUsers.Count;
            var firstPagesCount = PaginatorDto<User>.GetPagesTotalCount(firstUsersCount, getUsersQuery1.PageSize);

            firstReqUsers = firstReqUsers
               .Skip((getUsersQuery1.PageNumber - 1) * getUsersQuery1.PageSize)
               .Take(getUsersQuery1.PageSize)
               .ToList();

            var secondReqUsers = new20Users
                .Where(x => x.Login.Contains(getUsersQuery2.SearchLoginTerm))
                .Where(x => x.Email.Contains(getUsersQuery2.SearchEmailTerm))
                .OrderByDescending(x => x.Email)
                .ToList();

            var secondUsersCount = secondReqUsers.Count;
            var secondPagesCount = PaginatorDto<User>.GetPagesTotalCount(secondUsersCount, getUsersQuery2.PageSize);

            secondReqUsers = secondReqUsers
               .Skip((getUsersQuery2.PageNumber - 1) * getUsersQuery2.PageSize)
               .Take(getUsersQuery2.PageSize)
               .ToList();

            var usersUrl = _usersAbsUrl.Substring(0, _usersAbsUrl.Length - 1);

            var query1 = TestData.GetAbsUrlWithQuery(_usersAbsUrl, getUsersQuery1);
            var query2 = TestData.GetAbsUrlWithQuery(_usersAbsUrl, getUsersQuery2);

            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Get, query1, null);
            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Get, query2, null);

            // Act
            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var httpResponse2 = await _client.GetAsync(query1);
            var httpResponse3 = await _client.GetAsync(query2);

            var httpResponse4 = await _client.SendAsync(requestMessage2);
            var paginatorUsers1 = await httpResponse4.Content.ReadFromJsonAsync<PaginatorDto<ViewUserDto>>();

            var httpResponse5 = await _client.SendAsync(requestMessage3);
            var paginatorUsers2 = await httpResponse5.Content.ReadFromJsonAsync<PaginatorDto<ViewUserDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse3?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse4?.StatusCode);

            Assert.Equal(firstPagesCount, paginatorUsers1?.PagesCount);
            Assert.Equal(getUsersQuery1.PageNumber, paginatorUsers1?.Page);
            Assert.Equal(getUsersQuery1.PageSize, paginatorUsers1?.PageSize);
            Assert.Equal(firstUsersCount, paginatorUsers1?.TotalCount);

            Assert.Equal(firstReqUsers.Count, paginatorUsers1?.Items.Count);
            Assert.Equal(firstReqUsers.Last().Login, paginatorUsers1?.Items.Last().Login);
            Assert.Equal(firstReqUsers.Last().Email, paginatorUsers1?.Items.Last().Email);

            Assert.Equal(HttpStatusCode.OK, httpResponse5?.StatusCode);

            Assert.Equal(secondPagesCount, paginatorUsers2?.PagesCount); // err
            Assert.Equal(getUsersQuery2.PageNumber, paginatorUsers2?.Page);
            Assert.Equal(getUsersQuery2.PageSize, paginatorUsers2?.PageSize);
            Assert.Equal(secondUsersCount, paginatorUsers2?.TotalCount);

            Assert.Equal(secondReqUsers.Count, paginatorUsers2?.Items.Count);
            Assert.Equal(secondReqUsers.Last().Login, paginatorUsers2?.Items.Last().Login);
            Assert.Equal(secondReqUsers.Last().Email, paginatorUsers2?.Items.Last().Email);
        }

        [Fact]
        public async Task Test02_CreateUser()
        {
            // Arrange
            var createdTime = DateTime.UtcNow;

            var inputUserDto = TestData.GetCorrectInputUserDto();
            var badInputUserDto = TestData.GetIncorrectInputUserDto();

            var requestMessage = TestData.GetAuthorizedMessage(HttpMethod.Post, _usersAbsUrl, badInputUserDto);
            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Post, _usersAbsUrl, inputUserDto);
            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Get, _usersAbsUrl, null);

            // Act
            var httpResponse1 = await _client.PostAsJsonAsync(_usersRelUrl, inputUserDto);

            var httpResponse2 = await _client.SendAsync(requestMessage);

            var httpResponse3 = await _client.SendAsync(requestMessage2);
            var user = await httpResponse3.Content.ReadFromJsonAsync<ViewUserDto>();

            var timeDiff = user?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            var httpResponse4 = await _client.SendAsync(requestMessage3);
            var usersDb = await httpResponse4.Content.ReadFromJsonAsync<PaginatorDto<ViewUserDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse1?.StatusCode);

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse2?.StatusCode);

            Assert.Equal(HttpStatusCode.Created, httpResponse3?.StatusCode);
            Assert.Equal(inputUserDto.Login, user?.Login);
            Assert.Equal(inputUserDto.Email, user?.Email);
            Assert.True(isLessThan2Min);

            Assert.Equal(HttpStatusCode.OK, httpResponse4?.StatusCode);
            Assert.Equal(1, usersDb?.Items.Count);
            Assert.Equal(inputUserDto.Login, usersDb?.Items.Last().Login);
            Assert.Equal(inputUserDto.Email, usersDb?.Items.Last().Email);
        }

        [Fact]
        public async Task Test03_DeleteUser()
        {
            // Arrange
            var inputUserDto = TestData.GetCorrectInputUserDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, _usersAbsUrl, inputUserDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var user = await httpResponse1.Content.ReadFromJsonAsync<ViewUserDto>();

            var requestMessage2 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_usersAbsUrl}999", null);
            var requestMessage3 = TestData.GetAuthorizedMessage(HttpMethod.Delete, $"{_usersAbsUrl}{user?.Id}", null);
            var requestMessage4 = TestData.GetAuthorizedMessage(HttpMethod.Get, _usersAbsUrl, null);

            // Act

            var httpResponse2 = await _client.SendAsync(requestMessage2);

            var httpResponse3 = await _client.DeleteAsync($"{_usersRelUrl}{user?.Id}");

            var httpResponse4 = await _client.SendAsync(requestMessage3);
            var httpResponse5 = await _client.SendAsync(requestMessage4);
            var usersDb = await httpResponse5.Content.ReadFromJsonAsync<PaginatorDto<ViewUserDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse4?.StatusCode);

            Assert.Equal(HttpStatusCode.OK, httpResponse5?.StatusCode);
            Assert.Equal(0, usersDb?.Items.Count);
        }
    }
}
