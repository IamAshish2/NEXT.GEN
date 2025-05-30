using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.FriendshipDto;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipsRepository _friendshipsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FriendshipController(IFriendshipsRepository friendshipsRepository,IUserRepository userRepository, IMapper mapper)
        {
            _friendshipsRepository = friendshipsRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("user/add-friend")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddFriend([FromBody] CreateFriendshipDto addNewFriend)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var checkFriendshipExists = await _friendshipsRepository.CheckIfFriendshipAlreadyExists(addNewFriend.RequestorUserId,addNewFriend.RequestedUserId);
                if (checkFriendshipExists)
                {
                    return BadRequest("The friendship already exists.");
                }

                var getRequestor = await _userRepository.GetUserByNameAsync(addNewFriend.RequestorUserId);
                var getRequested = await _userRepository.GetUserByNameAsync(addNewFriend.RequestedUserId);

                if(getRequestor == null || getRequested == null)
                {
                    return NotFound();
                }

                var friendshipMap = _mapper.Map<Friends>(addNewFriend);

                if(! await _friendshipsRepository.AddFriend(friendshipMap))
                {
                    ModelState.AddModelError("","Server is busy. Try again later.");
                    return BadRequest(ModelState);
                }

                return Ok("new Friend added.");

            }
            catch (System.Exception)
            {

                throw;
            }
        }


        [HttpGet("get-users-friends/{userId}")]
        public async Task<ActionResult<ICollection<GetUsersFriendshipsDto>>> GetUsersFriends(string userId )
        {
            try
            {
                var userExists = await _userRepository.checkIfUserExists(userId);
                if (!userExists)
                {
                    return BadRequest();
                }

                var friendships = await _friendshipsRepository.GetUsersFriends(userId);
                if(friendships == null)
                {
                    return Ok();
                }

                return friendships;
            }
            catch (System.Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("remove-friend")]
        public async Task<ActionResult> RemoveFriend([FromBody] RemoveFriendDto removeFriend)
        {
            try
            {
                var getUser = await _userRepository.GetUserByNameAsync(removeFriend.RequestorUserName);
                var getAnotherUser = await _userRepository.GetUserByNameAsync(removeFriend.RequestedUserName);

                if(getUser == null || getAnotherUser == null)
                {
                    return BadRequest();
                }

                var friendship = await _friendshipsRepository.GetFriendship(removeFriend.RequestorUserName,removeFriend.RequestedUserName);

                if(! await _friendshipsRepository.RemoveFriend(friendship))
                {
                    return BadRequest("The server is busy. Please try again later.");
                }

                return Ok("User successfully removed.");
            }
            catch (System.Exception ex)
            {
                return BadRequest();
            }
        }

    }
}
