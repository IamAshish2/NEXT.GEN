using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIConcepts.Models
{
    public class User
    {
        [Key]
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty!;

        // some user details
        public string? Bio {  get; set; }
        // the course the student is studying
        public string? Course { get; set; }
        public string? Address {  get; set; }
        public ICollection<string>? Socials { get; set; } = new List<string>();
        public ICollection<string>? Skills { get; set; } = new List<string>();

        // relationships
        public ICollection<CreatePost> Posts { get; set; } = new List<CreatePost>();
        public ICollection<Likes> Likes { get; set; } = new List<Likes>();
        public ICollection<Dislike> Dislikes { get; set; } = new List<Dislike>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        // the friends of the currently logged in user user1
        public ICollection<Friendships> UserFriendships { get; set; } = new List<Friendships>();

        // the friends of the user's friend, if the currently logged in user has a friend, then the friend also has current user as friend
        public ICollection<Friendships> FriendsFriendships { get; set; } = new List<Friendships>();
        // one user can join multiple groups and be a Group member
        public ICollection<GroupMembers> GroupMember { get; set; } = new List<GroupMembers>();

        // one user can create multiple groups
        public ICollection<Group> CreatedGroups { get; set; } = new List<Group>();
    }
}
