using NEXT.GEN.Dtos.UserDto;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    public class GetFriendshipsDto
    {
        public int FriendshipId { get; set; }
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime FriendshipDate { get; set; }
        public GetUserDto Friend { get; internal set; }
    }
}
