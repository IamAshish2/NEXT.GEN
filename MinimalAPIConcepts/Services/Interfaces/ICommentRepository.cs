using NEXT.GEN.Dtos.CommentDto;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface ICommentRepository
    {
        Task<bool> CreateComment(Comment newComment);
        Task<bool> SaveChanges();
        Task<bool> DeleteComment(Comment comment);
        Task<ICollection<GetPostCommentsDto>> GetPostComments(int postId);
    }
}
