using MinimalAPIConcepts.Models;

namespace MinimalAPIConcepts.Services.Interfaces
{
    public interface IUserRepository
    {
        Task<ICollection<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<bool> CreateUserAsync(User newUser);
        Task<bool> UpdateUserAsync(Guid userId, User user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> isEmailInUse(string Email);
        Task<bool> isUserNameInUse(string userName);
        Task<bool> checkIfUserExists(Guid userId);
        Task<bool> SaveAsync();
    }
}
