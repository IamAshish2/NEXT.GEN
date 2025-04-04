using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.PostModel
{
    public class CreatePost
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        // one user can make many posts.
        public string CreatorId { get; set; }
        [ForeignKey("CreatorId")]
        public User User { get; set; }

        // one group can have many posts
        public string GroupName { get; set; }
        [ForeignKey("GroupName")]
        public Group Group { get; set; }

        // one post can have many likes, dislikes and comments.
        public ICollection<Likes> Likes { get; set; } = new List<Likes>();
        public ICollection<Dislike> Dislikes { get; set; } = new List<Dislike>();
        public ICollection<Comment> Comment { get; set; } = new List<Comment>();
    }
}

/*
 * createPost(int userId, [FromBody] CreatePostDto] -> post
 * updatePost(int postId, [FromBody] updatedPostDto] -> put
 * deletePost (int postId)
 * getAllUsersPost(int userId)
 * 
 * when i click on some user's profile, how will i render their data and their posts? -> getPostByName(string userName) ?
 */