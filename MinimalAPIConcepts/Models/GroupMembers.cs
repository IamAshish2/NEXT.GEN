using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models
{
    public class GroupMembers
    {
        [Key]
        public int GroupMemberId { get; set; }
        public DateTime JoinDate { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int GroupId {  get; set; }
        [ForeignKey("GroupId")]
        public Group Group { get; set; }
    }
}
