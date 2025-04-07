using NEXT.GEN.Dtos.PostDto;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IPostRepository
    {
        // get all posts by all users
        Task<ICollection<CreatePost>> GetAllPosts();
        // get all post by a certain user
        Task<ICollection<CreatePost>> GetPostsByUser(string userName);
        Task<bool> CreatePost(CreatePost newPost);
        Task<bool> DeletePost(CreatePost post);
        Task<bool> UpdatePost(CreatePost post);
        Task<bool> SaveChanges();

        Task<bool> DoesPostExist(int postId);



        // Post interface for groups
        Task<ICollection<GetGroupPostsDto>> GetAllPostsFromGroup(string groupName,string userId);
        Task<GetGroupPostsDto> GetPostDetailsById(int postId);
    }
}
