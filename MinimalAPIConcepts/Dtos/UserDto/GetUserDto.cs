using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.UserDto
{
    public class GetUserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
