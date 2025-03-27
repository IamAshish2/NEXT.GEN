using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Dtos.PostDto;
using NEXT.GEN.Models;
using NEXT.GEN.Models.PostModel;
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
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IMapper _mapper;

        public GroupsController(IGroupRepository groupRepository,IGroupMemberRepository groupMemberRepository, IMapper mapper)
        {
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _mapper = mapper;
        }

        // GET: api/groups
        [HttpGet("get-all-groups")]
        public async Task<ActionResult<ICollection<GetGroupDetailsDto>>> GetAllGroups()
        {
            try
            {
                var groups = await _groupRepository.GetAllGroups();
                
                if(groups == null)
                {
                    return NotFound();
                }

                return Ok(groups);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /*
        [HttpGet("get-by-groupName/{groupName}")]
        public async Task<ActionResult<GetGroupDetailsDto>> GetGroupByName(string groupName)
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
        */


        [HttpGet("get-by-groupName")]
        public async Task<ActionResult<GetGroupDetailsDto>> GetGroupByName([FromQuery] GetGroupMembersDto query)
        {
            try
            {
                var group = await _groupRepository.GetGroupDetailsByName(query.GroupName,query.UserName);


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
                //var group = new Group
                //{
                //    GroupName = createGroupDto.GroupName,
                //    Description = createGroupDto.Description,
                //    CreatorName = createGroupDto.CreatorName,
                //    //Members = new List<GroupMembers> { new GroupMembers { UserId = createGroupDto.CreatorId} }
                //};

                var group = _mapper.Map<Group>(createGroupDto);

                group.MemberCount++;

                var created = await _groupRepository.CreateGroup(group);
                if (!created)
                    return StatusCode(500, "Something went wrong while creating the group.");

                // check this
                /*
                 * when a user creates a group, he/she should automatically be the group member and the stattus should be of joined.
                 * does the below assignment does the task of adding and joining the creator of the group?
                 */
                //group.Members = new List<GroupMembers> { new GroupMembers { UserName = createGroupDto.CreatorName, GroupName = createGroupDto.GroupName } };

                var newMember = _mapper.Map<GroupMembers>(group);
                newMember.UserName = group.CreatorName;

                if (!await _groupMemberRepository.JoinGroup(newMember))
                {
                    ModelState.AddModelError("error", "The request could not be processed at the moment.");
                    return BadRequest(ModelState);
                }

                if (!await _groupMemberRepository.MakeUserAMember(group.GroupName, group.CreatorName))
                {
                    ModelState.AddModelError("error", "The user add process failed.");
                    await _groupMemberRepository.RemoveMember(newMember);
                    return BadRequest(ModelState);
                }


                await _groupRepository.Save();

                return CreatedAtAction(nameof(GetGroupByName), new { group.GroupName }, group);
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

        [HttpPost("upload-post-to-group")]
        public async Task<IActionResult> UploadPostToGroup([FromBody] UploadPostToGroup request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if(! await _groupRepository.DoesGroupExist(request.GroupName)){
                return NotFound();
            }

            if(!await _groupMemberRepository.DoesUserExists(request.UserName)){
                return NotFound();
            }

            if(!await _groupMemberRepository.IsUserAlreadyAMember(request.UserName, request.GroupName))
            {
                return BadRequest("The user is not a part of the group.");
            }

            var mappedRequest = _mapper.Map<CreatePost>(request);

            if(!await _groupRepository.CreatePost(mappedRequest))
            {
                ModelState.AddModelError("","The post was not created");
                return BadRequest();
            }

            return NoContent();
        }
    }
}
