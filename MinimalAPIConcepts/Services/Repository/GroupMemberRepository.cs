using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Models;
using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // this is the joinGroup 
        public async Task<bool> AddMember(GroupMembers groupMembers)
        {
            await _context.GroupMembers.AddAsync(groupMembers);
            return await Save();
        }

        /*
         * This method returns all the group members of a group for a given groupId
         *  -----------------------------------------------------------------------
         *  make this method take the groupName, which you should make unique and get the groupmembers via groupName instead, that will be easier to do.
         */
        public async Task<List<GroupMembers>> GetGroupMembers(string groupName)
        {
            return await _context.GroupMembers.OrderBy(gm => gm.GroupMemberId)
                .Include(g => g.User)
                .Where(g => g.Group.GroupName == groupName).ToListAsync();

            //var group = await _context.Groups
            //.Include(g => g.Members)
            //.ThenInclude(gm => gm.User)
            //.FirstOrDefaultAsync(g => g.GroupId == groupId);
            //return group;
        }

        // this is the Leave group

        public Task<bool> RemoveMember(GroupMembers groupMember)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        /*
         * 
         * Task<bool> GroupExists(int groupId);
         */

        //public async Task<bool> GroupExists(int groupId)
        //{
        //    var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        //    return group != null;
        //}

        public async Task<bool> GroupExists(string groupName)
        {
            var group =  await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
            return group != null;
        }

        public async Task<bool> IsUserAlreadyAMember(int userId,string groupName)
        {
            var userExists = await _context.GroupMembers.FirstOrDefaultAsync(g => g.UserId == userId && g.GroupName == groupName);
            return userExists != null;
        }
    }

}
