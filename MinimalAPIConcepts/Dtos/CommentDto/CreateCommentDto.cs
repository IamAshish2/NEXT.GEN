using MinimalAPIConcepts.Models;
using NEXT.GEN.Models.PostModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Dtos.CommentDto
{
    public class CreateCommentDto
    {
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public int PostId { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
