using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.UserDto;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    // dto to get all the friends of a user
    public class GetUsersFriendshipsDto
    {
        public int FriendshipId { get; set; }
        public int FriendId { get; set; }
        public GetUserDto Friend { get; set; }
    }
}
