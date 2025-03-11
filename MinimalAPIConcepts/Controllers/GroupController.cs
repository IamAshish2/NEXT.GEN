using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

// This controller handles the actions performed for a group
// handling group memebers joining and leaving is handled by the group member entity


namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        // GET: api/groups
        [HttpGet("get-all-groups")]
        public async Task<ActionResult<ICollection<Group>>> GetAllGroups()
        {
            try
            {
                var groups = await _groupRepository.GetAllGroups();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/groups/{id}
        //[HttpGet("get-by-id/{id}")]
        //public async Task<ActionResult<Group>> GetGroupById(int id)
        //{
        //    try
        //    {
        //        var group = await _groupRepository.GetGroupById(id);

        //        if (group == null)
        //            return NotFound("Group not found.");

        //        return Ok(group);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred: {ex.Message}");
        //    }
        //}

        [HttpGet("get-by-groupName/{groupName}")]
        public async Task<ActionResult<Group>> GetGroupByName(string groupName)
        {
            try
            {
                //var group = await _groupRepository.GetGroupById(id);
                var group = await _groupRepository.GetGroupByName(groupName);

                if (group == null)
                    return NotFound("Group not found.");

                return Ok(group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/groups
        [HttpPost("create-group")]
        public async Task<ActionResult> CreateGroup([FromBody] CreateGroupDto createGroupDto)
        {
            try
            {
                if (createGroupDto == null)
                    return BadRequest("Invalid group data.");

                 //Check if the user (creator) exists
                //var userExists = await _context.Users.AnyAsync(u => u.Id == createGroupDto.CreatorId);
                //if (!userExists)
                //    return NotFound("User not found.");

                // Map DTO to Group model
                var group = new Group
                {
                    GroupName = createGroupDto.GroupName,
                    Description = createGroupDto.Description,
                    CreatorId = createGroupDto.CreatorId,
                    Members = new List<GroupMembers> { new GroupMembers { UserId = createGroupDto.CreatorId} }
                };

                var created = await _groupRepository.CreateGroup(group);
                if (!created)
                    return StatusCode(500, "Something went wrong while creating the group.");

                return CreatedAtAction(nameof(GetGroupByName), new { GroupName = group.GroupName }, group);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // PUT: api/groups/{id}
        [HttpPut("update-group/{groupName}")]
        public async Task<ActionResult> UpdateGroup(string groupName, [FromBody] Group updatedGroup)
        {
            try
            {
                if (updatedGroup == null || groupName != updatedGroup.GroupName)
                    return BadRequest("Invalid group data.");

                var existingGroup = await _groupRepository.GetGroupByName(groupName);
                if (existingGroup == null)
                    return NotFound("Group not found.");

                var updated = await _groupRepository.UpdateGroup(updatedGroup);
                if (!updated)
                    return StatusCode(500, "Something went wrong while updating the group.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // DELETE: api/groups/{id}
        [HttpDelete("delete-group/{groupName}")]
        public async Task<ActionResult> DeleteGroup(string groupName)
        {
            try
            {
                var exists = await _groupRepository.DoesGroupExist(groupName);
                if (!exists)
                    return NotFound("Group not found.");

                var deleted = await _groupRepository.DeleteGroup(groupName);
                if (!deleted)
                    return StatusCode(500, "Something went wrong while deleting the group.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        //[HttpPost]

    }
}
