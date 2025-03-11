using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.GroupMembersDto
{
    public class GetGroupMembersDTO
    {
        public DateTime JoinDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GroupId { get; set; }
    }
}
