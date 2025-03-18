using System.ComponentModel.DataAnnotations;

namespace MinimalAPIConcepts.Dtos.UserDto
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Course { get; set; }
        public string? Address { get; set; }
        public ICollection<string>? Socials { get; set; } = new List<string>();
        public ICollection<string>? Skills { get; set; } = new List<string>();
    }
}
