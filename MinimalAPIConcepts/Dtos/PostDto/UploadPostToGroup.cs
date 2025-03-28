namespace NEXT.GEN.Dtos.PostDto
{
    public class UploadPostToGroup
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public string UserName { get; set; }
        public string GroupName { get; set; }
    }
}
