using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;

namespace MinimalAPIConcepts.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateUserAsync(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            return await SaveAsync();
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {

                _context.Users.Remove(user);
            }
            return await SaveAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.Id).ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Update(user);
            return await SaveAsync();
        }

        public async Task<bool> isEmailInUse(string Email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            return user == null ? false : true;
        }

        public async Task<bool> isUserNameInUse(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            return user == null ? false : true;
        }
        public async Task<bool> checkIfUserExists(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? false : true;
        }
    }
}
