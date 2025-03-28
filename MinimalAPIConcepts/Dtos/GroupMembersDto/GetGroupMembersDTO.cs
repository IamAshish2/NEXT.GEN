using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.UserDto;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.GroupMembersDto
{
    public class GetGroupMembersDTO
    {
        public DateTime JoinDate { get; set; }
        //public string userName { get; set; }
        public bool HasJoined { get; set; }
        public string userName { get; set; }
        public GetUserDto User { get; set; }

        // we can omit the group Name here.
        public string GroupName { get; set; }
    }
}
