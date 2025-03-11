using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.PostDto
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public int UserId { get; set; }
    }
}
