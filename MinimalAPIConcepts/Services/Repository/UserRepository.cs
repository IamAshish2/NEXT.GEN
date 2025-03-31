using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Exception;
using NEXT.GEN.Exceptions;
using System.Security.Claims;

namespace MinimalAPIConcepts.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;

        public UserRepository(ApplicationDbContext context,ITokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<bool> CreateUserAsync(User newUser)
        {
            await _context.Users.AddAsync(newUser);
            return await SaveAsync();
        }

        public async Task<bool> DeleteUserAsync(string UserName)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == UserName);
            if (user != null)
            {

                _context.Users.Remove(user);
            }
            return await SaveAsync();
        }

        public async Task<User> GetUserByNameAsync(string UserName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == UserName);
        }

        public async Task<ICollection<User>> GetUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.UserName).ToListAsync();
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
        public async Task<bool> checkIfUserExists(string UserName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == UserName);
            return user == null ? false : true;
        }

        public async Task<string> LoginWithGoogle(ClaimsPrincipal? claimsPrincipal)
        {
            if(claimsPrincipal == null)
            {
                throw new ExternalLoginProviderException("Google","Claims principal is null");
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? throw new ExternalLoginProviderException("Google", "Email is null");

            if (email == null)
            {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }


            var user = await isEmailInUse(email);

            if (!user)
            {
                var newUser = new User
                {
                    FullName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                    UserName = claimsPrincipal.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                    Email = email,
                };

                var result = await CreateUserAsync(newUser);

                if (!result)
                {
                    throw new ExternalLoginProviderException("Google","The user could not be created at the moment.");
                }
            }

            var userName = claimsPrincipal.FindFirstValue(ClaimTypes.Name);

            var jwtToken = _tokenGenerator.GenerateToken(userName,email);
            return jwtToken;
        }
    }
}
