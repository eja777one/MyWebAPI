using MyWebAPI.Models.Blogs;
using Assert = Xunit.Assert;

namespace MyWebApiTests.Unit
{
    public class BlogUnitTests
    {
        [Fact]
        public void CreateBlog()
        {
            // Arrange
            var dto = TestData.GetCorrectInputBlogDto();
            var createdTime = DateTime.UtcNow;

            // Act

            var blog = new Blog(dto);

            var timeDiff = blog?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            // Assert

            Assert.Equal(dto.Name, blog?.Name);
            Assert.Equal(dto.Description, blog?.Description);
            Assert.Equal(dto.WebsiteUrl, blog?.WebsiteUrl);
            Assert.False(blog?.IsMembership);
            Assert.True(isLessThan2Min);
        }

        [Fact]
        public void UpdateBlog()
        {
            // Arrange
            var dto = TestData.GetIncorrectInputBlogDto();

            // Act
            var blog = new Blog();
            blog.Update(dto);

            // Assert
            Assert.Equal(dto.Name, blog?.Name);
            Assert.Equal(dto.Description, blog?.Description);
            Assert.Equal(dto.WebsiteUrl, blog?.WebsiteUrl);
            Assert.False(blog?.IsMembership);
        }
    }
}
