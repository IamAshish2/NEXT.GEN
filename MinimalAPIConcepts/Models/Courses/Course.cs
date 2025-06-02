using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Models.UserModel;

namespace NEXT.GEN.Models.Courses;

public class Course
{
    [Key]
    public int CourseId { get; set; }
    public required string CourseName { get; set; } 
    public required string Description { get; set; }
    // who uploaded this course
    [ForeignKey("UserId")]
    [Required]
    public string UploadedBy { get; set; }
    public virtual User User { get; set; }
    
    // one course has multiple enrollments
    public ICollection<Enrollment> Enrollments = new List<Enrollment>();
}