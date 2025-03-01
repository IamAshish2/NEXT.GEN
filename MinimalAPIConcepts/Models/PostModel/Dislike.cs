using Microsoft.Identity.Client;
using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models.PostModel
{
    public class Dislike
    {
        [Key]
        public int DislikeId { get; set; }
        public int UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }
        public DateTime DislikeDate {  get; set; }
    }
}
