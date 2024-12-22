using MyWebAPI.Dto.User;
using MyWebAPI.Repositories;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Services.Interfaces;

namespace MyWebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly PasswordService _passwordService;

        public UsersService(IUsersRepository usersRepository, PasswordService passwordService)
        {
            _usersRepository = usersRepository;
            _passwordService = passwordService;
        }

        public async Task<ViewUserDto?> AddUser(InputUserDto dto)
        {
            var users = await _usersRepository.GetUsersWithSameLoginOrEmail(dto);
            if (users.Count > 0) return null;

            var data = _passwordService.GenerateData(dto.Password);

            var user = await _usersRepository.AddUser(new(dto, data.salt, data.hash));

            return user is null ? null : new(user.Id, user.Login, user.Email, user.CreatedAt);
        }

        public async Task<bool> Login(LoginInputDto dto)
        {
            var user = await _usersRepository.GetUser(dto.LoginOrEmail);
            if (user is null) return false;

            return _passwordService.CheckData(dto, user);
        }
    }
}
