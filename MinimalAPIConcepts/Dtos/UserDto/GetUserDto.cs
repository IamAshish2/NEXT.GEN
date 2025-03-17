using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.UserDto
{
    public class GetUserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
