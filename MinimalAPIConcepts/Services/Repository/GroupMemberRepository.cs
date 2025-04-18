﻿using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Context;
using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Dtos.UserDto;
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
        public async Task<bool> JoinGroup(GroupMembers groupMembers)
        {
            await _context.GroupMembers.AddAsync(groupMembers);
            return await Save();
        }

        /*
         * This method returns all the group members of a group for a given groupId
         *  -----------------------------------------------------------------------
         *  make this method take the groupName, which you should make unique and get the groupmembers via groupName instead, that will be easier to do.
         */
        public async Task<List<GetGroupMembersDTO>> GetGroupMembers(string groupName)
        {
            return await _context.GroupMembers.OrderBy(gm => gm.GroupMemberId)
                .Include(g => g.User)
                .Where(g => g.Group.GroupName == groupName)
                .Select(g => new GetGroupMembersDTO
                {
                    JoinDate = g.JoinDate,
                    HasJoined = g.HasJoined,
                    GroupName = g.Group.GroupName,
                    userName = g.User.UserName,
                    User = new GetUserDto
                    {
                        UserName = g.User.UserName,
                        Email = g.User.Email,
                        Course = g.User.Course
                    }
                }).ToListAsync();
        }

                  

            //var group = await _context.Groups
            //.Include(g => g.Members)
            //.ThenInclude(gm => gm.User)
            //.FirstOrDefaultAsync(g => g.GroupId == groupId);
            //return group

        // this is the Leave group

        public async Task<bool> RemoveMember(GroupMembers groupMember)
        {
            _context.GroupMembers.Remove(groupMember);
            return await Save();
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

        public async Task<bool> IsUserAlreadyAMember(string userId,string groupName)
        {
            var userExists = await _context.GroupMembers.FirstOrDefaultAsync(g => g.MemberId == userId && g.GroupName == groupName);
            return userExists != null;
        }

        public async Task<bool> MakeUserAMember(string groupName,string userId)
        {
            var user = await _context.GroupMembers.FirstOrDefaultAsync(g => g.MemberId == userId && g.GroupName == groupName);

            if(user != null)
            {

                user.HasJoined = true;
            }
            return await Save();
        }

        public async Task<bool> DoesUserExists(string userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }
    }

}
