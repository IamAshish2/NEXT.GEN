using System.ComponentModel.DataAnnotations;

namespace MinimalAPIConcepts.Dtos.UserDto
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
