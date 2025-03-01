using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int MemberCount { get; set; } = 0;
        public int Description { get; set; }
        // the creator of the group 
        public int CreatorId { get; set; }
        public User User { get; set; }

        // one group can have multiple group members
        public ICollection<GroupMembers> Members { get; set; } = new List<GroupMembers> { };
    }
}
