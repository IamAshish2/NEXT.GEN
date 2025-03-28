using NEXT.GEN.Dtos.LikeDto;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface ILikeRepository
    {
        // add new row of data if user likes the post
        Task<bool> AddLikeToPost(Likes newLike);
        // essentially delete the row of data if the post is disliked by the user
        Task<bool> CheckIfLiked(AddLikeDto request);
        Task<bool> RemoveLikeToPost(Likes oldLike);
        Task<Likes> FindLike(AddLikeDto request);
        Task<bool> Save();
    }
}
