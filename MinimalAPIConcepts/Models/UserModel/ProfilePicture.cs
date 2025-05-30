using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Models;

public class ProfilePicture
{
    [Key]
    public int Id { set; get; }
    public string UserId { set; get; }
    public required string Url { set; get; }
    public required string Description { set; get; }
    public DateTime DateAdded { set; get; }
    public bool IsMain { get; set; }
    public required string PublicId { get; set; }
}