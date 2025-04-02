using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace NEXT.GEN.Dtos.UserDto
{
    public class TokenDto
    {
        public string Token { get; set; }  
        public string RefreshToken { get; set; }
        //public string UserName { get; set; }
    }
}
