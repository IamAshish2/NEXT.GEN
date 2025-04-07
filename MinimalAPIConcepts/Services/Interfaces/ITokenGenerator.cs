using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(User user);
        //ClaimsPrincipal ValidateToken(string token);
        Task<string> GenerateRefreshToken();
        Task InsertRefreshToken(string userName, string refreshToken);
    }
}
