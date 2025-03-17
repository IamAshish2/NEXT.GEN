using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.UserDto
{
    public class EditUserDetailsDTO
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? Course { get; set; }
        public string? Address { get; set; }
        public ICollection<string>? Socials { get; set; } = new List<string>();
        public ICollection<string>? Addresses { get; set; } = new List<string>();
    }
}
