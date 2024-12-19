using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Blogs;

namespace MyWebApiTests
{
    public class TestData
    {
        public static InputBlogDto GetCorrectInputBlogDto()
        {
            var rnd = new Random();

            var name = $"Name_{rnd.Next(1, 1000)}";
            var description = $"Description_{rnd.Next(1, 1000)}";
            var website = $"https://site_{rnd.Next(1, 1000)}.com";

            return new InputBlogDto(name, description, website);
        }

        public static InputBlogDto GetIncorrectInputBlogDto()
        {
            var rnd = new Random();

            var description = $"Description_{rnd.Next(1, 1000)}";
            var website = $"https://site_{rnd.Next(1, 1000)}.com";

            return new InputBlogDto(description, website);
        }

        public static InputPostDto GetCorrectInputPostDto(int blogId)
        {
            var rnd = new Random();

            var title = $"Title_{rnd.Next(1, 1000)}";
            var shortDescription = $"ShortDescription_{rnd.Next(1, 1000)}";
            var content = $"Content{rnd.Next(1, 1000)}";

            return new InputPostDto(title, shortDescription, content, blogId);
        }

        public static InputPostDto GetIncorrectInputPostDto()
        {
            var rnd = new Random();

            var title = $"Title_{rnd.Next(1, 1000)}";
            var shortDescription = $"ShortDescription_{rnd.Next(1, 1000)}";
            var content = $"Content_{rnd.Next(1, 1000)}";

            return new InputPostDto(title, shortDescription, content);
        }

        public static HttpRequestMessage GetAuthorizedMessage(HttpMethod method, string url, object body)
        {
            var str = "admin:admin";
            var bytes = Encoding.UTF8.GetBytes(str);

            var token = Convert.ToBase64String(bytes);

            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", token);
            requestMessage.Content = JsonContent.Create(body);

            return requestMessage;
        }
    }
}
