namespace NEXT.GEN.Dtos.CommentDto
{
    public class GetPostCommentsDto
    {
        public int CommentId { get; set; }
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
