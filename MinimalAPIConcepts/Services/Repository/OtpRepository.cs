using Microsoft.Extensions.Caching.Memory;
using NEXT.GEN.Dtos.Otp;
using NEXT.GEN.Services.Interfaces;
using System.Security.Cryptography;

namespace NEXT.GEN.Services.Repository
{
    public class OtpRepository(IMemoryCache cache) : IOtpRepository
    {
        public Task deleteOtp(string email)
        {
            cache.Remove(email + "_otp");
            cache.Remove(email + "_user");
            return Task.CompletedTask;
        }

        public Task<string> GenerateRandomOtp()
        {

            var random = RandomNumberGenerator.Create();
            int length = 6;
            string allowedChars = "1234567890";
            byte[] bytes = new byte[length];
            random.GetBytes(bytes);
            char[] code = new char[bytes.Length];

            for(int i=0; i<length;i++)
            {
                code[i] = allowedChars[bytes[i] % allowedChars.Length];
            }

            return Task.FromResult(new string(code));
        }

        public Task storeOtpForResetingPassword(string email, string otp)
        {
            // the otp service is only availabe for 5 minutes
            cache.Set(email + "_forgotPasswordServiceOtp" , otp, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task<string> getStoredOtpAsync(string email)
        {
            cache.TryGetValue(email + "_otp", out string otp);
            return Task.FromResult(otp);

        }

        public async Task<CreateUserDto> getStoredUserDto(string email)
        {
            cache.TryGetValue(email + "_user", out CreateUserDto request);

            return  await Task.FromResult(request);
        }

        public Task storeOtp(string email, string otp, CreateUserDto request)
        {
            cache.Set(email + "_otp",otp, TimeSpan.FromMinutes(5));
            cache.Set(email + "_user",request,TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }
    }
}
