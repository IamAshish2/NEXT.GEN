using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IpostRepository
    {
        // get all posts by all users
        Task<ICollection<Post>> GetAllPosts();
        // get all post by a certain user
        Task<ICollection<Post>> GetPostsByUser(int userId);
        Task<bool> CreatePost(Post newPost);
        Task<bool> DeletePost(Post post);
        Task<bool> UpdatePost(Post post);
        Task<bool> SaveChanges();
    }
}
