using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using MyWebAPI.Models.User;

namespace MyWebApiTests.Unit
{
    public class UserUnitTests
    {
        [Fact]
        public void CreateUser()
        {
            // Arrange
            var inputUserDto = TestData.GetCorrectInputUserDto();
            var createdTime = DateTime.UtcNow;

            // Act
            var user = new User(inputUserDto, "salt", "hash");

            var timeDiff = user?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            // Assert
            Assert.Equal(inputUserDto.Login, user?.Login);
            Assert.Equal(inputUserDto.Email, user?.Email);
            Assert.True(isLessThan2Min);
        }
    }
}
