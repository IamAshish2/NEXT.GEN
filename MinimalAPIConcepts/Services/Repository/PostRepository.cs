using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using NEXT.GEN.Dtos.CommentDto;
using NEXT.GEN.Dtos.PostDto;
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

        public async Task<bool> DoesPostExist(int postId)
        {
            return await _context.Posts.AnyAsync(p => p.PostId == postId);
        }

        public async Task<ICollection<CreatePost>> GetAllPosts()
        {
            return await _context.Posts.OrderBy(p => p.PostId).ToListAsync();
        }

        public async Task<ICollection<GetGroupPostsDto>> GetAllPostsFromGroup(string groupName,string userId)
        {
            return await _context.Posts
                .Where(p => p.GroupName == groupName)
                .OrderByDescending(p => p.PostId)
                .Include(p => p.Comment)
                .Select(p => new GetGroupPostsDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Description = p.Description,
                    PostedDate = p.PostedDate,
                    ImageUrls = p.ImageUrls,
                    userId = p.CreatorId,
                    userName = p.User.UserName,
                    // if any row matches the groupname of a post, then consider it liked.
                    // say i am retrieving a list of posts, and i want to see if the posts are liked
                    // every post check, if the row contains the groupname,with the postId, then it is liked.
                    IsLiked = p.Likes.Any(l => l.GroupName == groupName && l.PostId == p.PostId && l.UserId == userId),
                    LikeCount = p.Likes.Count(l => l.GroupName == groupName && l.PostId == p.PostId),
                    Comments = p.Comment.Select(c => new GetPostCommentsDto
                    {
                        CommentId = c.CommentId,
                        Content = c.Content,
                        userId = c.UserId,
                        CommentDate = c.CommentDate
                    }).ToList()
                })
                .ToListAsync();
        }

        // This method returns a collection of 
        public async Task<GetGroupPostsDto> GetPostDetailsById(int postId)
        {
            return await _context.Posts.Where(p => p.PostId == postId)
                .Include(p => p.Comment)
                .Select(p => new GetGroupPostsDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Description = p.Description,
                    PostedDate = p.PostedDate,
                    ImageUrls = p.ImageUrls,
                    userId = p.CreatorId,

                    IsLiked = p.Likes.Any(l => l.GroupName == p.GroupName && l.PostId == p.PostId),
                    LikeCount = p.Likes.Count(l => l.GroupName == p.GroupName && l.PostId == p.PostId),
                    Comments = p.Comment.Select(c => new GetPostCommentsDto
                    {
                        CommentId = c.CommentId,
                        Content = c.Content,
                        userId = c.UserId,
                        userName = c.User.UserName,
                        CommentDate = c.CommentDate
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<ICollection<CreatePost>> GetPostsByUser(string userId)
        {
            return await _context.Posts.OrderBy(p => p.PostId).Where(p => p.CreatorId == userId).ToListAsync();
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
