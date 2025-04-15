using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Dtos.GroupDto
{
    public class GetGroupDetailsDto
    {
        public string GroupName { get; set; }
        public int MemberCount { get; set; } = 0;
        public string Description { get; set; }
        public string Category { get; set; }
        public string GroupImage { get; set; }
        //public string CreatorName { get; set; }
        public string CreatorId { get; set; }
        // check if the currently logged in user is memeber of the group he/she is visiting
        public bool HasJoined { get; set; }
    }
}
