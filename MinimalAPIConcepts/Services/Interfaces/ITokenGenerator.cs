namespace MinimalAPIConcepts.Services.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateToken(string userName, string email);
    }
}
