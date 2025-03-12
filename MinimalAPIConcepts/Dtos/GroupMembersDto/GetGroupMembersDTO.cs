using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.UserDto;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.GroupMembersDto
{
    public class GetGroupMembersDTO
    {
        public DateTime JoinDate { get; set; }
        public int UserId { get; set; }
        public GetUserDto User { get; set; }
        
        // we can omit the group Name here.
        public string GroupName { get; set; }
    }
}
