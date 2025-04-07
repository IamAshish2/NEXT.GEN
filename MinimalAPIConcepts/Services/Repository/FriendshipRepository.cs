using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Context;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class FriendshipRepository : IFriendshipsRepository
    {
        private readonly ApplicationDbContext _context;

        public FriendshipRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddFriend(Friendships newFriend)
        {
            await _context.Friends.AddAsync(newFriend);
            return await Save();
        }

        public async Task<bool> CheckIfFriendshipAlreadyExists(string  requestorUserId, string  requestedUserId)
        {

            var checkFriendship = await _context.Friends.Where(f => f.UserId == requestedUserId && f.FriendId == requestedUserId).FirstOrDefaultAsync();
            if(checkFriendship != null)
            {
                return  true;
            }

            return false;
        }

        public async Task<Friendships> GetFriendship(string requestedUserId, string requestorUserId)
        {
            return await _context.Friends.Where(f => f.UserId == requestedUserId && f.FriendId == requestorUserId).FirstOrDefaultAsync();
        }

        // what am i gonna do with get all friendships?
        public async Task<ActionResult<ICollection<Friendships>>> GetAllFriendships()
        {
            return  await _context.Friends.OrderBy(f => f.FriendshipId).ToListAsync();
        }

        public async Task<ActionResult<ICollection<GetUsersFriendshipsDto>>> GetUsersFriends(string userId)
        {
            var friends = await _context.Friends
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.UserId)
                .Include(f => f.Friend)
                .Select(f => new GetUsersFriendshipsDto
                {
                    FriendshipId = f.FriendshipId,
                    //userName = f.UserName,
                    Friend = new GetUserDto
                   {
                        UserName = f.Friend.UserName,
                        Email   = f.Friend.Email,
                   }

                })
                .ToListAsync();

            return  friends;
        }

        public async Task<bool> RemoveFriend(Friendships friend)
        {   
             _context.Friends.Remove(friend);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
