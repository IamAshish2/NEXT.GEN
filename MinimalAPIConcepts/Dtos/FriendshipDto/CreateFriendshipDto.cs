using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    public class CreateFriendshipDto
    {
        public string RequestorUserName { get; set; }
        public string RequestedUserName { get; set; }
        public DateTime FriendshipDate { get; set; }
    }
}
