using NEXT.GEN.Dtos.Otp;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IOtpRepository
    {
        Task<string> GenerateRandomOtp();
        // for use while creating a new user
        Task storeOtp(string email, string otp, CreateUserDto request);
        // for use while creating a new user
        
        
        // for use when the user requests forget password service
        Task storeOtpForResetingPassword(string email, string otp);
        Task<string> getStoredOtpForResettingPassword(string email);
        Task<string> getStoredOtpAsync(string email);
        Task<CreateUserDto> getStoredUserDto(string email);
        Task deleteOtp(string email);
    }
}
