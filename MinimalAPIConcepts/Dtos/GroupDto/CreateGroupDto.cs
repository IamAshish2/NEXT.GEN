using NEXT.GEN.Models;

namespace NEXT.GEN.Dtos.GroupDto
{
    public class CreateGroupDto
    {
        public string GroupName { get; set; }  
        public string Description { get; set; }  
        public string Category { get; set; }
        public string GroupImage { get; set; }
        //public string? CreatorName { get; set; }
        public string? CreatorId { get; set; }
    }
}
