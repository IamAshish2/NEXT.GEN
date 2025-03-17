using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class PostRepository : IpostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreatePost(CreatePost newPost)
        {
            await _context.Posts.AddAsync(newPost);
            return await SaveChanges();

        }

        public async Task<bool> DeletePost(CreatePost post)
        {
            _context.Posts.Remove(post);
            return await SaveChanges();
        }

        public async Task<ICollection<CreatePost>> GetAllPosts()
        {
            return await _context.Posts.OrderBy(p => p.PostId).ToListAsync();
        }

        public async Task<ICollection<CreatePost>> GetPostsByUser(string userName)
        {
            return await _context.Posts.OrderBy(p => p.PostId).Where(p => p.UserName == userName).ToListAsync();
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePost(CreatePost post)
        {
            _context.Posts.Update(post);
            return await SaveChanges();
        }
    }
}
