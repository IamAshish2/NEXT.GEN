using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.UserModel.PostModel
{
    public class Dislike
    {
        [Key]
        public int DislikeId { get; set; }
        public string UserName {  get; set; }
        [ForeignKey("UserName")]
        public User User { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public CreatePost Post { get; set; }
        public DateTime DislikeDate {  get; set; }
    }
}
