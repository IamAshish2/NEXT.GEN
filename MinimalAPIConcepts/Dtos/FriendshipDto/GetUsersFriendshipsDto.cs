using NEXT.GEN.Dtos.UserDto;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    // dto to get all the friends of a user
    public class GetUsersFriendshipsDto
    {
        public int FriendshipId { get; set; }
        //public string userName { get; set; }
        public GetUserDto Friend { get; set; }
    }
}
