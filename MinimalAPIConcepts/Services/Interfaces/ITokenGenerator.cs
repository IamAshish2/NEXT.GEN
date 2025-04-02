using MinimalAPIConcepts.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MinimalAPIConcepts.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
        //ClaimsPrincipal ValidateToken(string token);
        Task<string> GenerateRefreshToken();
        Task InsertRefreshToken(string userName, string refreshToken);
    }
}
