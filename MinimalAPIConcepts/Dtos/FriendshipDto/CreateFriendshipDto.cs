using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.FriendshipDto
{
    public class CreateFriendshipDto
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public DateTime FriendshipDate { get; set; }
    }
}
