﻿using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IFriendshipsRepository
    {
        Task<ActionResult<ICollection<Friendships>>> GetAllFriendships();
        // get all friends of the user
        Task<ActionResult<ICollection<GetUsersFriendshipsDto>>> GetUsersFriends(string UserName);
        // add friend for a user

        Task<bool> CheckIfFriendshipAlreadyExists(string requestorUserName,string requestedUserName);
        Task<bool> AddFriend(Friendships newFriend);
        // remove an existing friend of a user
        Task<bool> RemoveFriend(Friendships friend);
        // get the whole friendship row from the table.
        Task<Friendships> GetFriendship(string requestorUserName, string requestedUserName);
        Task<bool> Save();
    }
}
