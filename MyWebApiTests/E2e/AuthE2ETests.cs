using System.Net.Http.Json;
using System.Net;
using MyWebAPI.Dto.User;

namespace MyWebApiTests.E2e
{
    [Collection("Auth")]
    public class AuthE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;

        private readonly string _authRelUrl = "/auth/login";

        private string _usersAbsUrl = "";
        private readonly string _usersRelUrl = "/users/";

        public AuthE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("https://localhost");
            _usersAbsUrl = $"{_client.BaseAddress}{_usersRelUrl.Substring(1)}";
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public async Task DisposeAsync() => await _factory.ResetDatabaseAsync();

        [Fact]
        public async Task Test01_Login()
        {
            // Arrange
            var inputUserDto = TestData.GetCorrectInputUserDto();
            var requestMessage1 = TestData.GetAuthorizedMessage(HttpMethod.Post, $"{_usersAbsUrl}", inputUserDto);

            var httpResponse1 = await _client.SendAsync(requestMessage1);
            var viewUser = await httpResponse1.Content.ReadFromJsonAsync<ViewUserDto>();

            var correctLoginDto1 = new LoginInputDto() { LoginOrEmail = inputUserDto.Login, Password = inputUserDto.Password };
            var correctLoginDto2 = new LoginInputDto() { LoginOrEmail = inputUserDto.Email, Password = inputUserDto.Password };

            var inCorrectLoginDto1 = new LoginInputDto() { LoginOrEmail = inputUserDto.Email, Password = "blabla" };
            var inCorrectLoginDto2 = new LoginInputDto() { LoginOrEmail = inputUserDto.Email };

            // Act
            var httpResponse2 = await _client.PostAsJsonAsync(_authRelUrl, inCorrectLoginDto1);
            var httpResponse3 = await _client.PostAsJsonAsync(_authRelUrl, inCorrectLoginDto2);

            var httpResponse4 = await _client.PostAsJsonAsync(_authRelUrl, correctLoginDto1);
            var httpResponse5 = await _client.PostAsJsonAsync(_authRelUrl, correctLoginDto2);

            // Assert
            Assert.Equal(HttpStatusCode.Created, httpResponse1?.StatusCode);
            Assert.Equal(inputUserDto.Login, viewUser?.Login);
            Assert.Equal(inputUserDto.Email, viewUser?.Email);

            Assert.Equal(HttpStatusCode.Unauthorized, httpResponse2?.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse3?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse4?.StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse5?.StatusCode);
        }
    }
}
