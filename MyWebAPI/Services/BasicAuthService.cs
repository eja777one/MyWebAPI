using MyWebAPI.Dto.User;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Services
{
    public class BasicAuthService : IBasicAuthService
    {
        // In-memory list of users.
        private List<UserDto> users = new List<UserDto>
            {
                // Initial set of users. Use hashed passwords in a production environment.
                new UserDto { Id = 1, Username = "admin", Password = "admin" },
                new UserDto { Id = 2, Username = "user", Password = "user" },
                new UserDto { Id = 3, Username = "Pranaya", Password = "Test@1234" },
                new UserDto { Id = 4, Username = "Kumar", Password = "Admin@123" }
            };

        // Validates user credentials against the stored list.
        public async Task<UserDto?> ValidateUser(string username, string password)
        {
            await Task.Delay(100); // Simulates a delay, mimicking database latency.
            return users.FirstOrDefault(u => u.Username == username && u.Password == password); // Returns the user if credentials match.
        }

        // Retrieves all users.
        public async Task<List<UserDto>> GetAllUsers()
        {
            await Task.Delay(100); // Simulates a delay.
            return users.ToList(); // Converts the list of users to a new list and returns it.
        }

        // Retrieves a user by ID.
        public async Task<UserDto?> GetUserById(int id)
        {
            await Task.Delay(100); // Simulates a delay.
            return users.FirstOrDefault(u => u.Id == id); // Returns the user if found.
        }

        // Adds a new user if no duplicate ID is found.
        public async Task<UserDto> AddUser(UserDto user)
        {
            await Task.Delay(100); // Simulates a delay.
            if (users.Any(u => u.Id == user.Id))
            {
                throw new Exception("User already exists with the given ID."); // Exception if user with same ID exists.
            }

            users.Add(user); // Adds the new user to the list.
            return user; // Returns the added user.
        }

        // Updates an existing user's details.
        public async Task<UserDto> UpdateUser(UserDto user)
        {
            await Task.Delay(100); // Simulates a delay.
            var existingUser = await GetUserById(user.Id); // Fetches the user by ID.
            if (existingUser == null)
            {
                throw new Exception("User not found."); // Throws an exception if user not found.
            }

            existingUser.Username = user.Username; // Updates username.
            existingUser.Password = user.Password; // Updates password, consider hashing in production.
            return existingUser; // Returns the updated user.
        }

        // Deletes a user by ID.
        public async Task DeleteUser(int id)
        {
            await Task.Delay(100); // Simulates a delay.
            var user = await GetUserById(id); // Fetches the user by ID.
            if (user == null)
            {
                throw new Exception("User not found."); // Throws an exception if user not found.
            }

            users.Remove(user); // Removes the user from the list.
        }
    }

}
