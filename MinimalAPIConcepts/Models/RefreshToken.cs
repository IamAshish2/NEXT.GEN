using MinimalAPIConcepts.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        // one user can have multiple refresh tokens
        public string userName { get; set; }
        //[ForeignKey(nameof(UserId))]
        //public User User { get; set; }  
        public string Token { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
