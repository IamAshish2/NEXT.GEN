using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.PostModel
{
    public class Post
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
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        // one post can have many likes, dislikes and comments.
        public ICollection<Likes> Likes { get; set; } = new List<Likes>();
        public ICollection<Dislike> Dislikes { get; set; } = new List<Dislike>();
        public ICollection<Comment> Comment { get; set; } = new List<Comment>();
    }
}
