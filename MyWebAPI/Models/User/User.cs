using MyWebAPI.Dto.User;

namespace MyWebAPI.Models.User
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = null!; // unique
        public string Email { get; set; } = null!; // unique
        public DateTime CreatedAt { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;

        public User() { }
        public User(InputUserDto dto, string salt, string hash)
        {
            Login = dto.Login;
            Email = dto.Email;
            PasswordHash = hash;
            PasswordSalt = salt;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
