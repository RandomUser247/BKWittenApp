using BackendServer.Services.Interfaces;
using ContentDB.Migrations;
using Microsoft.EntityFrameworkCore;

namespace BackendServer.Services
{
    public class UserService : IUserService
    {
        private readonly ContentDBContext _context;

        public UserService(ContentDBContext context)
        {
            _context = context;
        }

        // Get a user by their email address
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Get all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync();
        }

        // Get a user by their ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }
    }
}
