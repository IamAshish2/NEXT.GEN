using MinimalAPIConcepts.Models;
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
        public string CreatorName { get; set; }
        [ForeignKey("CreatorName")]
        public User User { get; set; }

        // one group can have multiple group members
        public ICollection<GroupMembers> Members { get; set; } = new List<GroupMembers> { };
    }
}
