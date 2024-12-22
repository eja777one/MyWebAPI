using MyWebAPI.Data;
using MyWebAPI.Dto;
using MyWebAPI.Dto.User;
using MyWebAPI.Models.User;
using MyWebAPI.Repositories.Interfaces;
using MyWebAPI.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly MainDbContext _context;

        public UsersRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AddUser(User user)
        {
            await _context.Users.AddAsync(user);
            return await SaveChanges() ? user : null;
        }

        public async Task<List<User>?> AddUsers(List<User> users)
        {
            await _context.Users.AddRangeAsync(users);
            return await SaveChanges() ? users : null;
        }

        public async Task<bool> DeleteAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            _context.Users.RemoveRange(users);
            return await SaveChanges();
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null) return false;

            _context.Remove(user);
            return await SaveChanges();
        }

        public async Task<User?> GetUser(string loginOrEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Login == loginOrEmail || x.Email == loginOrEmail);
        }

        public async Task<PaginatorDto<ViewUserDto>> GetUsers(GetUsersQueryDto dto)
        {
            var users = await _context.Users
                .Where(x => x.Login.Contains(dto.SearchLoginTerm))
                .Where(x => x.Email.Contains(dto.SearchEmailTerm))
                .OrderByColumn(dto.SortBy, dto.SortDirection)
                .ToListAsync();

            var usersView = users.Select(x => new ViewUserDto(x.Id, x.Login, x.Email, x.CreatedAt)).ToList();

            return new PaginatorDto<ViewUserDto>(usersView, dto);
        }

        public async Task<List<User>> GetUsersWithSameLoginOrEmail(InputUserDto dto)
        {
            var users = await _context.Users
                .Where(x => x.Login == dto.Login)
                .Where(x => x.Email == dto.Email)
                .ToListAsync();

            return users;
        }

        private async Task<bool> SaveChanges()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }
}
