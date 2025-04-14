using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    public class CreateFriendshipDto
    {
        public string RequestorUserId { get; set; }
        public string RequestedUserId { get; set; }
        public DateTime FriendshipDate { get; set; }
    }
}
