using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NEXT.GEN.Models;

public class ProfilePicture
{
    [Key]
    public int Id { set; get; }
    [Required]
    public string UserId { set; get; }
    [ForeignKey("UserId")]
    // virtual access modifier for lazy loading support
    public virtual User User { get; set; }
    public required string Url { set; get; }
    public required string Description { set; get; }
    public DateTime DateAdded { set; get; }
    public bool IsMain { get; set; }
    public required string PublicId { get; set; }
}