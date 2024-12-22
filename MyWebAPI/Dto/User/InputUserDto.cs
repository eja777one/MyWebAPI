using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.User
{
    public class InputUserDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        [RegularExpression("^[a-zA-Z0-9_-]*$")]
        public string Login { get; set; } = null!; // must be unique!

        [Required]
        [MinLength(6)]
        [MaxLength(20)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; } = null!; // must be unique!

        public InputUserDto() { }

        public InputUserDto(string login, string password, string email)
        {
            Login = login;
            Password = password;
            Email = email;
        }
    }
}
