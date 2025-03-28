using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using NEXT.GEN.Dtos.LikeDto;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _context;

        public LikeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddLikeToPost(Likes newLike)
        {
            await _context.Likes.AddAsync(newLike);
            return await Save();
        }

        public async Task<Likes> FindLike(AddLikeDto request)
        {
            return await _context.Likes.FirstOrDefaultAsync(l => l.PostId == request.PostId && request.UserName == l.UserName && request.GroupName == l.GroupName);
        }

        public async Task<bool> CheckIfLiked(AddLikeDto request)
        {
            return await _context.Likes.AnyAsync(l => l.PostId == request.PostId && request.UserName == l.UserName && request.GroupName == l.GroupName);
        }

        public async Task<bool> RemoveLikeToPost(Likes oldLike)
        {
            _context.Likes.Remove(oldLike);
            return await Save();    
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
