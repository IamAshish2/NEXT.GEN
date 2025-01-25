using MinimalAPIConcepts.Models;

namespace MinimalAPIConcepts.Interfaces
{
    public interface IUserRepository
    {
        // Read Operations
        Task<IEnumerable<User>> GetUsersAsync(); 
        Task<User> GetUserByIdAsync(Guid userId); 

        // Create Operation
        Task<bool> CreateUserAsync(User newUser); 

        // Update Operation
        Task<bool> UpdateUserAsync(Guid userId, User user); 

        // Delete Operation
        Task<bool> DeleteUserAsync(Guid userId); 

        // Existence Check
        Task<bool> EmailExists(string Email);

        Task<bool> checkIfUserExists(Guid userId);

        // Save Changes
        Task<bool> SaveAsync(); 
    }
}
