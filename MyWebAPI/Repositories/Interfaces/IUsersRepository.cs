using MyWebAPI.Dto;
using MyWebAPI.Dto.User;
using MyWebAPI.Models.User;

namespace MyWebAPI.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<PaginatorDto<ViewUserDto>> GetUsers(GetUsersQueryDto dto);
        Task<List<User>> GetUsersWithSameLoginOrEmail(InputUserDto dto);
        Task<User?> GetUser(string loginOrEmail);
        Task<User?> AddUser(User user);
        Task<List<User>?> AddUsers(List<User> users);
        Task<bool> DeleteUser(int id);
        Task<bool> DeleteAllUsers();
    }
}
