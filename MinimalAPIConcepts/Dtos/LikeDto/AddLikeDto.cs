namespace NEXT.GEN.Dtos.LikeDto
{
    public class AddLikeDto
    {
        public string UserName { get; set; }    
        public int PostId { get; set; }

        // group name is optional because the user can post specific to their own profile as well,
        // if user is interacting with group posts, then the groupname is required
        // else, it is not.
        public string? GroupName { get; set; }
    }
}
