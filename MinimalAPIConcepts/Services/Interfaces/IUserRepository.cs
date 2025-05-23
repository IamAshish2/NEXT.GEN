﻿using System.Security.Claims;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsersAsync();
        Task<GetUserDto> GetUserByIdAsync(string userId);
        Task<User> GetUserByNameAsync(string userName);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User newUser);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string UserName);
        Task<bool> ChangePasswordAsync(string email, string password);
        Task<bool> isEmailInUse(string Email);
        Task<bool> isUserNameInUse(string userName);
        Task<bool> checkIfUserExists(string UserName);
        Task<bool> SaveAsync();
        // check the authentication state of the user
        ClaimsPrincipal validateJWT(string token);
        Task<User> LoginWithGoogle(ClaimsPrincipal? claimsPrincipal);
    }
}
