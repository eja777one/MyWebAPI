namespace MyWebAPI.Dto.User
{
    public class ViewUserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ViewUserDto() { }
        public ViewUserDto(int id, string login, string email, DateTime createdAt)
        {
            Id = id;
            Login = login;
            Email = email;
            CreatedAt = createdAt;
        }
    }
}
