using NEXT.GEN.Models;

namespace NEXT.GEN.Dtos.GroupDto
{
    public class CreateGroupDto
    {
        public string GroupName { get; set; }  
        public string Description { get; set; }  
        public int CreatorId { get; set; }
    }
}
