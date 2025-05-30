using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IFriendshipsRepository
    {
        Task<ActionResult<ICollection<Friends>>> GetAllFriendships();
        // get all friends of the user
        Task<ActionResult<ICollection<GetUsersFriendshipsDto>>> GetUsersFriends(string userId);
        // add friend for a user

        Task<bool> CheckIfFriendshipAlreadyExists(string requestorUserId,string requestedUserId);
        Task<bool> AddFriend(Friends newFriend);
        // remove an existing friend of a user
        Task<bool> RemoveFriend(Friends friend);
        // get the whole friendship row from the table.
        Task<Friends> GetFriendship(string requestorUserId, string requestedUserId);
        Task<bool> Save();
    }
}
