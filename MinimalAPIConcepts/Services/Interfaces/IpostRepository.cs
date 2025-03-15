using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IpostRepository
    {
        // get all posts by all users
        Task<ICollection<CreatePost>> GetAllPosts();
        // get all post by a certain user
        Task<ICollection<CreatePost>> GetPostsByUser(int userId);
        Task<bool> CreatePost(CreatePost newPost);
        Task<bool> DeletePost(CreatePost post);
        Task<bool> UpdatePost(CreatePost post);
        Task<bool> SaveChanges();
    }
}
