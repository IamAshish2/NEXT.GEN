using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IGroupRepository
    {
        Task<ICollection<GetGroupDetailsDto>> GetAllGroups();
        //Task<Group> GetGroupById(int groupId);
        Task<Group> GetGroupByName(string groupName);
        Task<bool> DoesGroupExist(string groupName);
        Task<bool> DeleteGroup(string groupName);

        //Task<bool> DoesGroupExist(int groupId);
        //Task<bool> DeleteGroup(int groupId);
        Task<bool> CreateGroup(Group group);
        // joinGroup controller is based on groupMember model, 
        //Task<bool> JoinGroup(int UserId, int GroupId);

        // getGroupMembers
        Task<bool> UpdateGroup(Group group);
        Task<bool> Save();
    }
}
