using System.ComponentModel.DataAnnotations;

namespace MyWebAPI.Dto.User
{
    public class LoginInputDto
    {
        [Required]
        public string LoginOrEmail { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
