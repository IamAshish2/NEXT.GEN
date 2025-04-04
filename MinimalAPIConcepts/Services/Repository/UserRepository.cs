using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentEmail.Core;

namespace MinimalAPIConcepts.Services.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;

        public UserRepository(ApplicationDbContext context,ITokenGenerator tokenGenerator,IOptions<JwtSettings> jwtSettings,IMapper mapper)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _jwtSettings = jwtSettings.Value;
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

        public async Task<GetUserDto> GetUserByIdAsync(string userId)
        {
            return await _context.Users.Where(u => u.Id == userId)
                .Select(u => new GetUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UserName = u.UserName,
                    Email = u.Email,
                    Bio = u.Bio,
                    Course = u.Course,
                    Address = u.Address,
                    Socials = u.Socials,
                    Skills = u.Skills,
                    Stats = new UserStatsResponseDto
                    {
                        Posts = u.Posts.Count(u => u.CreatorId == userId),
                        Groups = u.GroupMember.Count(u => u.MemberId == userId),
                        Connections = u.UserFriendships.Count(u => u.UserId == userId)
                    }
                }).FirstOrDefaultAsync();
                ;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }

            return user;

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
            _context.Users.Update(user);
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
        public async Task<bool> checkIfUserExists(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? false : true;
        }

        
        // TODO -> handle login for new user. dto mapping
        public async Task<string> LoginWithGoogle(ClaimsPrincipal? claimsPrincipal)
        {
            if(claimsPrincipal == null)
            {
                throw new ExternalLoginProviderException("Google","Claims principal is null");
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? throw new ExternalLoginProviderException("Google", "Email is null");

            // if the claimsPrincipal does not have email attached
            if (email == null)
            {
                throw new ExternalLoginProviderException("Google", "Email is null");
            }

            // check if we already have a user with the email, 
            // meaning the user is already a member of our platform
            var user = await GetUserByEmailAsync(email);

            // create a new user now
            if (user == null)
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
                
                var getUser = GetUserByEmailAsync(newUser.Email);
                var mapUser = _mapper.Map<User>(getUser);
                var jwt = _tokenGenerator.GenerateToken(mapUser);
                return jwt;
            }

            // for user already exists
            var userName = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            var mappedUser = _mapper.Map<User>(user);
            var jwtToken = _tokenGenerator.GenerateToken(mappedUser);
            return jwtToken;
        }

        public ClaimsPrincipal validateJWT(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // get the key that was used to create the token
            var securityKey = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,

                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(securityKey),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token,tokenValidationParameter,out var validatedToken);

            if(validatedToken is JwtSecurityToken)
            {
                return principal;
            }

            return null;
        }

        public async Task<User> GetUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
