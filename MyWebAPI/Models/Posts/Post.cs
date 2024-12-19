using MyWebAPI.Dto.Posts;

namespace MyWebAPI.Models.Posts
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public string Content { get; set; } = null!;
        public int BlogId { get; set; }

        public Post()
        {

        }

        public Post(InputPostDto dto)
        {
            Update(dto);
        }

        public void Update(InputPostDto dto)
        {
            Title = dto.Title;
            ShortDescription = dto.ShortDescription;
            Content = dto.Content;
            BlogId = (int)dto.BlogId;
        }
    }
}
