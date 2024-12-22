using MyWebAPI.Dto.User;
using MyWebAPI.Models.User;

namespace MyWebAPI.Services.Interfaces
{
    public interface IUsersService
    {
        Task<ViewUserDto?> AddUser(InputUserDto dto);
        Task<bool> Login(LoginInputDto dto);
    }
}
