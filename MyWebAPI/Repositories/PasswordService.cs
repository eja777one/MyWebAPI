using MyWebAPI.Dto.User;
using MyWebAPI.Models.User;

namespace MyWebAPI.Repositories
{
    public class PasswordService
    {
        public (string salt, string hash) GenerateData(string password)
        {
            var passwordSalt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, passwordSalt);

            return (passwordSalt, passwordHash);
        }

        public bool CheckData(LoginInputDto dto, User user)
        {
            var inputHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, user.PasswordSalt);
            return inputHash == user.PasswordHash;
        }
    }
}
