using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IGroupRepository
    {
        Task<ICollection<GetGroupDetailsDto>> GetAllGroups();
        //Task<Group> GetGroupById(int groupId);
        Task<GetGroupDetailsDto> GetGroupByName(string groupName);

        //  This endpoint returns gropDetails with hasJoined value for members
        Task<GetGroupDetailsDto> GetGroupDetailsByName(string groupName,string memberName);
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
        Task<bool> DoesUserExists(string userName);


        // create post in group.
        Task<bool> CreatePost(CreatePost post);
    }
}
