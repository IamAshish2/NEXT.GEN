using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace NEXT.GEN.Dtos.UserDto
{
    public class LoginResponseDto
    {
        public string Token { get; set; }   
        public int UserId { get; set; }
    }
}
