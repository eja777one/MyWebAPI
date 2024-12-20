using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using Assert = Xunit.Assert;

namespace MyWebApiTests.Unit
{
    public class PostUnitTests
    {
        [Fact]
        public void CreatePost()
        {
            // Arrange
            var inputBlogDto = TestData.GetCorrectInputBlogDto();
            var blog = new Blog(inputBlogDto);

            var dto = TestData.GetCorrectInputPostDto(blog.Id);
            var createdTime = DateTime.UtcNow;

            // Act
            var post = new Post(dto, blog.Name);

            var timeDiff = blog?.CreatedAt - createdTime;
            var isLessThan2Min = timeDiff?.Minutes < 2;

            // Assert
            Assert.Equal(dto.Title, post?.Title);
            Assert.Equal(dto.ShortDescription, post?.ShortDescription);
            Assert.Equal(dto.Content, post?.Content);
            Assert.Equal(dto.BlogId, post?.BlogId);
            Assert.Equal(blog?.Name, post?.BlogName);
            Assert.True(isLessThan2Min);
        }

        [Fact]
        public void UpdateBlog()
        {
            // Arrange
            var inputBlogDto1 = TestData.GetCorrectInputBlogDto();
            var blog1 = new Blog(inputBlogDto1);

            var inputBlogDto2 = TestData.GetCorrectInputBlogDto();
            var blog2 = new Blog(inputBlogDto2);

            var dto = TestData.GetCorrectInputPostDto(blog1.Id);
            var createdTime = DateTime.UtcNow;

            var updateDto = TestData.GetCorrectInputPostDto(blog2.Id);

            // Act
            var post = new Post(dto, blog1.Name);
            post.Update(updateDto, blog2.Name);

            // Assert
            Assert.Equal(updateDto.Title, post?.Title);
            Assert.Equal(updateDto.ShortDescription, post?.ShortDescription);
            Assert.Equal(updateDto.Content, post?.Content);
            Assert.Equal(updateDto.BlogId, post?.BlogId);
            Assert.Equal(blog2?.Name, post?.BlogName);
        }
    }
}
