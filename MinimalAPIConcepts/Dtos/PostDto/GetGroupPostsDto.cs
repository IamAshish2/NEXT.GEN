using System.ComponentModel.DataAnnotations;

namespace NEXT.GEN.Dtos.PostDto
{
    public class GetGroupPostsDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PostedDate { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string UserName { get; set; }
    }
}
