using MinimalAPIConcepts.Models;
using NEXT.GEN.Models.PostModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models
{
    public class Group
    {
        //public int GroupId { get; set; }
        // make groupName the primary key
        [Key]
        public string GroupName { get; set; }
        public int MemberCount { get; set; } = 0;
        public string Description { get; set; }
        // the creator of the group 
        public string Category { get; set; }
        public string GroupImage { get; set; }
        // for checking if the user has joined the  given group
        //public bool HasJoined { get; set; }
        //public string CreatorName { get; set; }
        //[ForeignKey("CreatorName")]

        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public User User { get; set; }

        // one group can have multiple group members
        public ICollection<GroupMembers> Members { get; set; } = new List<GroupMembers> { };
        // one group can have multiple posts i.e. users in a group can post multiple posts
        public ICollection<CreatePost> Posts { get; set; } = new List<CreatePost> { };
    }
}
