﻿using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Models;

namespace NEXT.GEN.Services.Interfaces
{
    public interface IGroupMemberRepository
    {
        Task<bool> AddMember(GroupMembers groupMembers);
        Task<bool> RemoveMember(GroupMembers groupMember);
        // get all the groupMembers
        Task<List<GroupMembers>> GetGroupMembers(string groupName);
        Task<bool> Save();


        //  group focused
        //Task<bool> GroupExists(int groupId);
        Task<bool> GroupExists(string groupName);

        Task<bool> IsUserAlreadyAMember(int userId,string groupName);
    }
}
