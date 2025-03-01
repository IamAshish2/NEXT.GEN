using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.UserDto;

namespace MinimalAPIConcepts.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> CreateUserAsync(User newUser);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> isEmailInUse(string Email);
        Task<bool> isUserNameInUse(string userName);
        Task<bool> checkIfUserExists(int userId);
        Task<bool> SaveAsync();
    }
}
