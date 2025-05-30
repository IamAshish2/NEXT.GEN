using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Models
{
    public class User : IdentityUser
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public override string? Email { get; set; }

        [Required, MaxLength(100)]
        public string Password { get; set; } = string.Empty!;

        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(100)]    
        public string? Course { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
        public ICollection<string>? Socials { get; set; } = new List<string>();
        public ICollection<string>? Skills { get; set; } = new List<string>();
        public ICollection<CreatePost> Posts { get; set; } = new List<CreatePost>();
        public ICollection<Likes> Likes { get; set; } = new List<Likes>();
        public ICollection<Dislike> Dislikes { get; set; } = new List<Dislike>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Friends> Friends { get; set; } = new List<Friends>();

        public ICollection<GroupMembers> GroupMembers { get; set; } = new List<GroupMembers>();

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
