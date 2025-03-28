using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using NEXT.GEN.Dtos.CommentDto;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class CommentRepository
        (
        ApplicationDbContext context
        ) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<bool> CreateComment(Comment newComment)
        {
            await _context.Comments.AddAsync(newComment);
            return await SaveChanges();
        }

        public async Task<bool> DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            return await SaveChanges();
        }

        //public async Task<ICollection<GetPostCommentsDto>> GetPostComments(int postId)
        //{
        //    return await _context.Posts.Where(p => p.PostId == postId)
        //        .Include(p => p.Comment)
        //        .Select(p => p.Comment.Select(c =>  new GetPostCommentsDto
        //        {
        //            CommentId = c.CommentId,
        //            CommentText = c.CommentText,
        //            UserName = c.UserName,
        //            CommentDate = c.CommentDate

        //        }).ToList()
        //        ).ToListAsync();
        //}
        //public int CommentId { get; set; }
        //public string CommentText { get; set; }
        //public string UserName { get; set; }
        //public DateTime CommentDate { get; set; }

        public async Task<ICollection<GetPostCommentsDto>> GetPostComments(int postId)
        {
            return await _context.Comments
                .Where(c => c.PostId == postId)
                .Select(c => new GetPostCommentsDto
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    UserName = c.UserName,
                    CommentDate = c.CommentDate
                })
                .ToListAsync();
        }


        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
