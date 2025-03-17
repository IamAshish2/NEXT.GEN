using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models
{
    // this table represents the friends the user can have
    // the intermediate table between users and users | one user can have many friends who are other users (self refrencing relationship)
    //[Index(nameof(UserId), nameof(FriendId), IsUnique = true)]
    public class Friendships
    {
        [Key]
        public int FriendshipId { get; set; }
        // the curent user
        public string UserName { get; set; }
        [ForeignKey("UserName")]
        // navigation property for the first user
        public User User { get; set; }

        // the user the current user is friends with
        public string AnotherUserName { get; set; }
        [ForeignKey("AnotherUserName")]
        // navigation property for the second user
        public User Friend { get; set; }    
        public DateTime FriendshipDate { get; set; }
    }
}


/*
 * how will add friend work?
 * the currently logged in user will click on [Add Friend] button
 * The second user will receive a request
 * If, accepted then, the user will be friend with the current user
 * addFriend(int userId, int secondUserId)
 */