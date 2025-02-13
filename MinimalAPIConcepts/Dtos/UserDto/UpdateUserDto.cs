using System.ComponentModel.DataAnnotations;

namespace MinimalAPIConcepts.Dtos.UserDto
{
    public class UpdateUserDto
    {
        [Required]
        public string? UserName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty!;
    }
}
