namespace NEXT.GEN.Dtos.GroupDto
{
    public class GetGroupDetailsDto
    {
        public string GroupName { get; set; }
        public int MemberCount { get; set; } = 0;
        public string Description { get; set; }
        public string Category { get; set; }
        public string GroupImage { get; set; }
        public string CreatorName { get; set; }
    }
}
