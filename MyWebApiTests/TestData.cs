﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using MyWebAPI.Dto;
using MyWebAPI.Dto.Blogs;
using MyWebAPI.Dto.Posts;
using MyWebAPI.Dto.User;
using MyWebAPI.Models.Blogs;
using MyWebAPI.Models.Posts;
using MyWebAPI.Models.User;
using MyWebAPI.Repositories;

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

        public static string GetAbsUrlWithQuery(string absUrl, SearchQueryDto dto)
        {
            var url = absUrl[..^1];

            url += $"?pageSize={dto.PageSize}";
            url += $"&pageNumber={dto.PageNumber}";
            url += $"&sortBy={dto.SortBy}";
            url += $"&sortDirection={dto.SortDirection}";

            if (dto is GetBlogsQueryDto bDto) url += $"&searchNameTerm={bDto.SearchNameTerm}";

            if (dto is GetUsersQueryDto uDto)
            {
                url += $"&searchLoginTerm={uDto.SearchLoginTerm}";
                url += $"&searchEmailTerm={uDto.SearchEmailTerm}";
            }

            return url;
        }

        public static List<User> GetUsers(int count, PasswordService? service)
        {
            var users = new List<User>();

            for (int i = 0; i < count; i++)
            {
                var dto = GetCorrectInputUserDto();

                if (i > -1 && i < 5) dto = GetCorrectInputUserDto(login: "Super", email: "Mega");
                if (i > 4 && i < 15) dto = GetCorrectInputUserDto(login: "Mega", email: "Super");
                if (i > 15 && i < count) dto = GetCorrectInputUserDto(login: "Perfect", email: "Mega");

                var data = service?.GenerateData(dto.Password);

                var salt = data is null ? "salt" : data.Value.salt;
                var hash = data is null ? "hash" : data.Value.hash;

                users.Add(new(dto, salt, hash));
            }

            return users;
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
        public static InputUserDto GetCorrectInputUserDto(string login = "Login", string pass = "Password",
            string email = "email")
        {
            var rnd = new Random();

            var uLogin = $"{login}_{rnd.Next(1, 1000)}";
            var uPassword = $"{pass}_{rnd.Next(1, 1000)}";
            var uEmail = $"https://{email}-{rnd.Next(1, 1000)}@test.com";

            return new InputUserDto(uLogin, uPassword, uEmail);
        }

        public static InputUserDto GetIncorrectInputUserDto(string login = "Login", string pass = "Password",
            string email = "email")
        {
            var rnd = new Random();

            var uLogin = $"{login}_{rnd.Next(1, 1000)}";
            var uPassword = $"{pass}_{rnd.Next(1, 1000)}";

            return new InputUserDto() { Login = uLogin, Password = uPassword };
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
