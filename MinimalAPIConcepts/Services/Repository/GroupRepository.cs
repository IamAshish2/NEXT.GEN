using Microsoft.EntityFrameworkCore;
using MinimalAPIConcepts.Context;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NEXT.GEN.Services.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateGroup(Group group)
        {
            await _context.Groups.AddAsync(group);
            return await Save();
        }

        //public async Task<bool> DeleteGroup(int groupId)
        //{
        //    var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        //    if (group == null)
        //        return false;

        //    _context.Groups.Remove(group);
        //    return await Save();
        //}


        //public async Task<bool> DoesGroupExist(int groupId)
        //{
        //    return await _context.Groups.AnyAsync(g => g.GroupId == groupId);
        //}

        public async Task<ICollection<GetGroupDetailsDto>> GetAllGroups()
        {
            return await _context.Groups.OrderByDescending(g => g.GroupName)
                .Select(g => new GetGroupDetailsDto
                {
                    GroupName = g.GroupName,
                    GroupImage = g.GroupImage,
                    Category = g.Category,
                    Description = g.Description,
                    MemberCount = g.MemberCount,
                    CreatorName = g.CreatorName,
                })
                .ToListAsync();
        }

        //public async Task<Group> GetGroupById(int groupId)
        //{
        //    return await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        //}

        public async Task<bool> UpdateGroup(Group group)
        {
            _context.Groups.Update(group);
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DoesGroupExist(string groupName)
        {
            return await _context.Groups.AnyAsync(g => g.GroupName == groupName);
        }

        public async Task<bool> DeleteGroup(string groupName)
        {
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupName == groupName);
            if (group == null)
                return false;

            _context.Groups.Remove(group);
            return await Save();
        }

        public async Task<GetGroupDetailsDto> GetGroupByName(string groupName)
        {
            return await _context.Groups.Where(g => g.GroupName == groupName)
                //.Select(g => g.Members.HasJoi)
                .Select(g => new GetGroupDetailsDto
                {
                    GroupName = g.GroupName,
                    GroupImage = g.GroupImage,
                    Category = g.Category,
                    Description = g.Description,
                    MemberCount = g.MemberCount,
                    CreatorName = g.CreatorName,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<GetGroupDetailsDto> GetGroupDetailsByName(string groupName, string memberName)
        {
            return await _context.Groups.Where(g => g.GroupName == groupName)
                .Select(g => new GetGroupDetailsDto
                {
                    GroupName = g.GroupName,
                    GroupImage = g.GroupImage,
                    Category = g.Category,
                    Description = g.Description,
                    MemberCount = g.MemberCount,
                    CreatorName = g.CreatorName,
                    HasJoined = g.Members.Any(g => g.UserName == memberName && g.GroupName == groupName && g.HasJoined),
                    
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> DoesUserExists(string userName)
        {
            return await _context.Users.AnyAsync(u => u.UserName == userName);
        }

        public async Task<bool> CreatePost(CreatePost post)
        {
            await _context.Posts.AddAsync(post);
            return await Save();
        }
    }
}
