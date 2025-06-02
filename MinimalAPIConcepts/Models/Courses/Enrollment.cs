using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Models.UserModel;

namespace NEXT.GEN.Models.Courses;

// join table between user and courses
[PrimaryKey(nameof(CourseId),nameof(UserId))]
public class Enrollment
{
    public int CourseId { get; set; }
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; }
    [MaxLength(450)]
    public required string UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}