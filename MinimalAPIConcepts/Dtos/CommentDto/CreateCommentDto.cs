using NEXT.GEN.Models.PostModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.CommentDto
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        //public string UserName { get; set; }
        public int PostId { get; set; }
        public DateTime? CommentDate { get; set; }
        // if the ParentId is 0, it means it is the top level comment
        // else it is a reply to a parent comment
        public int? ParentId { get; set; }
    }
}
