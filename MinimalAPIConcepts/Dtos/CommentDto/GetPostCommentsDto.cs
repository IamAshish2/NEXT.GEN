namespace NEXT.GEN.Dtos.CommentDto
{
    public class GetPostCommentsDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
