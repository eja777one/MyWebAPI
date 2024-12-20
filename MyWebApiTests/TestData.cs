using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;

namespace MyWebApiTests
{
    public class TestData
    {
        public static List<Blog> GetBlogs(int count)
        {
            var blogs = new List<Blog>();

            for (int i = 0; i < count; i++)
            {
                var dto = GetCorrectInputBlogDto();

                if (i > -1 && i < 5) dto = GetCorrectInputBlogDto(name: "Super");
                if (i > 4 && i < 15) dto = GetCorrectInputBlogDto(name: "Mega");
                if (i > 15 && i < count) dto = GetCorrectInputBlogDto(name: "Perfect");

                blogs.Add(new(dto));
            }

            return blogs;
        }

        public static List<Post> GetPosts(int count, int blogId, string blogName)
        {
            var posts = new List<Post>();

            for (int i = 0; i < count; i++)
            {
                var dto = GetCorrectInputPostDto(blogId);
                posts.Add(new(dto, blogName));
            }

            return posts;
        }

        public static InputBlogDto GetCorrectInputBlogDto(string name = "Name", string description = "Description",
            string site = "site")
        {
            var rnd = new Random();

            var bName = $"{name}_{rnd.Next(1, 1000)}";
            var bDescription = $"{description}_{rnd.Next(1, 1000)}";
            var website = $"https://{site}_{rnd.Next(1, 1000)}.com";

            return new InputBlogDto(bName, bDescription, website);
        }

        public static InputBlogDto GetIncorrectInputBlogDto()
        {
            var rnd = new Random();

            var description = $"Description_{rnd.Next(1, 1000)}";
            var website = $"https://site_{rnd.Next(1, 1000)}.com";

            return new InputBlogDto(description, website);
        }

        public static InputPostDto GetCorrectInputPostDto(int blogId, string title = "Title",
            string shortDescription = "ShortDescription", string content = "Content")
        {
            var rnd = new Random();

            var pTitle = $"{title}_{rnd.Next(1, 1000)}";
            var pShortDescription = $"{shortDescription}_{rnd.Next(1, 1000)}";
            var pContent = $"{content}_{rnd.Next(1, 1000)}";

            return new InputPostDto(pTitle, pShortDescription, pContent, blogId);
        }

        public static InputPostDto GetIncorrectInputPostDto()
        {
            var rnd = new Random();

            var title = $"Title_{rnd.Next(1, 1000)}";
            var shortDescription = $"ShortDescription_{rnd.Next(1, 1000)}";
            var content = $"Content_{rnd.Next(1, 1000)}";

            return new InputPostDto() { Title = title, ShortDescription = shortDescription, Content = content };
        }

        public static InputBlogPostDto GetCorrectInputBlogPostDto(string title = "Title",
            string shortDescription = "ShortDescription", string content = "Content")
        {
            var rnd = new Random();

            var pTitle = $"{title}_{rnd.Next(1, 1000)}";
            var pShortDescription = $"{shortDescription}_{rnd.Next(1, 1000)}";
            var pContent = $"{content}_{rnd.Next(1, 1000)}";

            return new InputBlogPostDto(pTitle, pShortDescription, pContent);
        }

        public static InputBlogPostDto GetIncorrectInputBlogPostDto()
        {
            var rnd = new Random();

            var shortDescription = $"ShortDescription_{rnd.Next(1, 1000)}";
            var content = $"Content_{rnd.Next(1, 1000)}";

            return new InputBlogPostDto() { ShortDescription = shortDescription, Content = content };
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
