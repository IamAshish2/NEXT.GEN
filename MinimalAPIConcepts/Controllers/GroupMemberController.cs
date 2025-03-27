using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GroupMembers> _logger;

        public GroupMemberController(IGroupMemberRepository groupMemberRepository, IGroupRepository groupRepository, IMapper mapper, ILogger<GroupMembers> logger)
        {
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;
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

            var userExists = await _groupMemberRepository.DoesUserExists(request.userName);
            if (!userExists)
            {
                return BadRequest();
            }

            // check if the group exists
            var groupExists = await _groupMemberRepository.GroupExists(request.GroupName);
            if (!groupExists)
                return BadRequest("The group does not exist.");

            // check if the user is already a memeber
            var isAMember = await _groupMemberRepository.IsUserAlreadyAMember(request.userName,request.GroupName);
            if (isAMember)
                return BadRequest("The user is already a part of the group.");

            // send a join request to the group for the given user
            var mappedRequest = _mapper.Map<GroupMembers>(request);
            mappedRequest.JoinDate = DateTime.Now;
            


            if (! await _groupMemberRepository.JoinGroup(mappedRequest)) {
                ModelState.AddModelError("error","The request could not be processed at the moment.");
                return BadRequest(ModelState);
            }

            if(!await _groupMemberRepository.MakeUserAMember(request.GroupName, request.userName))
            {
                ModelState.AddModelError("error", "The user add process failed.");
               await  _groupMemberRepository.RemoveMember(mappedRequest);
                return BadRequest(ModelState);
            }

           if(! await _groupRepository.UpdateMembers(mappedRequest.GroupName)){
                ModelState.AddModelError("error", "The group member status  could not be updated.");
                await _groupMemberRepository.RemoveMember(mappedRequest);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

       // //join group
       //[HttpPost("join-group")]
       // [ProducesResponseType(typeof(bool), 200)]
       // [ProducesResponseType(404)]
       // [ProducesResponseType(500)]
       // public async Task<ActionResult> JoinGroup([FromBody] JoinGroupDto joinGroup)
       // {
       //     try
       //     {
       //         var doesGroupExist = await _groupMemberRepository.GroupExists(joinGroup.GroupName);
       //         if (!doesGroupExist)
       //         {
       //             return NotFound("The group was not found.");
       //         }

       //         if(!await _groupMemberRepository.IsUserAlreadyAMember(joinGroup.userName, joinGroup.GroupName))
       //         {
       //             return BadRequest("User is already a member.");
       //         }

       //         var mappedRequest = _mapper.Map<GroupMembers>(joinGroup);
       //         mappedRequest.JoinDate = DateTime.Now;

       //         if(!await _groupMemberRepository.JoinGroup(mappedRequest))
       //         {
       //             ModelState.AddModelError("","An error occured while joining the group.");
       //             return BadRequest(ModelState);
       //         }

       //         if(!await _groupMemberRepository.MakeUserAMember(joinGroup.GroupName, joinGroup.userName))
       //         {
       //             await _groupMemberRepository.RemoveMember(mappedRequest);
       //             return BadRequest();
       //         }

       //         return Ok();

       //     }
       //     catch (Exception ex)
       //     {

       //         throw;
       //     }
       // }

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
