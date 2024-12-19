using MyWebAPI.Dto.User;

namespace MyWebAPI.Services.Interfaces
{
    public interface IBasicAuthService
    {
        Task<UserDto?> ValidateUser(string username, string password); // Method to validate a user's credentials.
        Task<List<UserDto>> GetAllUsers(); // Method to retrieve all users.
        Task<UserDto?> GetUserById(int id); // Method to fetch a single user by ID.
        Task<UserDto> AddUser(UserDto user); // Method to add a new user.
        Task<UserDto> UpdateUser(UserDto user); // Method to update an existing user's details.
        Task DeleteUser(int id); // Method to delete a user by ID.
    }
}
