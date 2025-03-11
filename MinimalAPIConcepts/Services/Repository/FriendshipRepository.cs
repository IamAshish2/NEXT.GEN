using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
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

        public async Task<bool> CheckIfFriendshipAlreadyExists(int requestorId, int requestedId)
        {

            var checkFriendship = await _context.Friends.Where(f => f.UserId == requestorId && f.FriendId == requestedId).FirstOrDefaultAsync();
            if(checkFriendship != null)
            {
                return  true;
            }

            return false;
        }

        public async Task<Friendships> GetFriendship(int userId, int anotherUserId)
        {
            var friendship = await _context.Friends.Where(f => f.UserId == userId && f.FriendId == anotherUserId).FirstOrDefaultAsync();
            return friendship;

        }

        // what am i gonna do with get all friendships?
        public async Task<ActionResult<ICollection<Friendships>>> GetAllFriendships()
        {
            return  await _context.Friends.OrderBy(f => f.FriendshipId).ToListAsync();
        }

        public async Task<ActionResult<ICollection<GetUsersFriendshipsDto>>> GetUsersFriends(int userId)
        {
            var friends = await _context.Friends
                .Where(f => f.UserId == userId)
                .OrderBy(f => f.UserId)
                .Include(f => f.Friend)
                .Select(f => new GetUsersFriendshipsDto
                {
                    FriendshipId = f.UserId,
                    FriendId = f.FriendId,
                    Friend = new GetUserDto
                   {
                        Id = f.Friend.Id,
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
