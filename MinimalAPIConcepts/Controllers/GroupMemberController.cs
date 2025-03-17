using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEXT.GEN.Dtos.GroupDto;
using NEXT.GEN.Dtos.GroupMembersDto;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupMembers> _logger;

        public GroupMemberController(IGroupMemberRepository groupMemberRepository, IMapper mapper, ILogger<GroupMembers> logger)
        {
            _groupMemberRepository = groupMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("get-group-members/{groupName}")]
        [ProducesResponseType(typeof(List<GetGroupMembersDTO>), 200)] //Documenting a successful 200 response.
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<GetGroupMembersDTO>>> GetGroupMembers(string groupName)
        {
            try
            {
                var doesGroupExist = await _groupMemberRepository.GroupExists(groupName);
                _logger.LogInformation("","The group exists or not?" + doesGroupExist);
                if (!doesGroupExist)
                {
                    return NotFound("The group was not found.");
                }

                var getGroupMembers = await _groupMemberRepository.GetGroupMembers(groupName);
                _logger.LogInformation(getGroupMembers + "--------------------------------------------------");
                if (getGroupMembers == null)
                {
                    return StatusCode(500);
                }

                return _mapper.Map<List<GetGroupMembersDTO>>(getGroupMembers);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error retrieving group members for group {GroupId}", groupId);

                return NotFound("The server sent error " + ex.Message) ;
                //return StatusCode(500, "An internal server error occurred.");
            }
        }


        // join a group
        [HttpPost("join-group")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> JoinGroup([FromBody] JoinGroupDto request)
        {
            if (!ModelState.IsValid) return BadRequest("The request was incomplete");

            // check if the group exists
            var groupExists = await _groupMemberRepository.GroupExists(request.GroupName);
            if (!groupExists)
                return BadRequest("The group does not exist.");

            // check if the user is already a memeber
            var isAMember = await _groupMemberRepository.IsUserAlreadyAMember(request.userName,request.GroupName);
            if (!isAMember)
                return BadRequest("The user is already a part of the group.");

            // send a join request to the group for the given user
            var mappedUser = _mapper.Map<GroupMembers>(request);

            if (! await _groupMemberRepository.JoinGroup(mappedUser)) {
                ModelState.AddModelError("error","The request could not be processed at the moment.");
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        // join group
        //[HttpPost("join-group")]
        //[ProducesResponseType(typeof(bool) , 200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult> JoinGroup([FromBody] JoinGroupDto joinGroup)
        //{
        //    try
        //    {
        //        var doesGroupExist = await _groupMemberRepository.GroupExists(joinGroup.GroupName);
        //        if (!doesGroupExist)
        //        {
        //            return NotFound("The group was not found.");
        //        }

        //        var doesUserExist = await _groupMemberRepository.IsUserAlreadyAMember(joinGroup.UserId,joinGroup.GroupName);


        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        //// Remove a Member (Admin Only)
        //[HttpDelete("remove/{userId}")]
        //public async Task<IActionResult> RemoveMember(int groupId, int userId)
        //{
        //    var groupMember = await _context.GroupMembers
        //        .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId);

        //    if (groupMember == null) return NotFound();

        //    _context.GroupMembers.Remove(groupMember);
        //    var group = await _context.Groups.FindAsync(groupId);
        //    if (group != null) group.MemberCount--;

        //    await _context.SaveChangesAsync();
        //    return Ok(new { message = "Member removed from group." });
        //}
    }
}
