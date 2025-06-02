using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.pagination;
using NEXT.GEN.Models.PostModel;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IGroupRepository
    {
         Task<bool> UpdateMembers(string groupName);
        Task<PaginatedList<Group>> GetAllGroups(int pageIndex, int pageSize);
        Task<GetGroupDetailsDto> GetGroupByName(string groupName);

        //  This endpoint returns gropDetails with hasJoined value for members
        Task<GetGroupDetailsDto> GetGroupDetailsByName(string groupName,string memberId);
        Task<bool> DoesGroupExist(string groupName);
        Task<bool> DeleteGroup(string groupName);

        Task<bool> CreateGroup(Group group);

        // getGroupMembers
        Task<bool> UpdateGroup(Group group);
        Task<bool> Save();
        Task<bool> DoesUserExists(string userName);


        // create post in group.
        Task<bool> CreatePost(CreatePost post);
    }
}
