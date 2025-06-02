using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.PostModel
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public CreatePost Post { get; set; }
        public DateTime CommentDate { get; set; }

        // comment chaining
        // foreign key
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        // navigation property
        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    }
}
