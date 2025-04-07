using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.LikeDto;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    // using primary constructor
    public class LikeController(
        ILikeRepository likeRepository,
        IMapper mapper,
        IGroupRepository groupRepository,
        IUserRepository userRepository,
        IPostRepository postRepository
            ) : ControllerBase
    {
        private readonly ILikeRepository _likeRepository = likeRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPostRepository _postRepository = postRepository;

        [HttpPost("like-post")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AddLike([FromBody] AddLikeDto request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("The model state was invalid.");
            }

            Request.Cookies.TryGetValue("userId",out var userId);

            if (String.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            if(!await _postRepository.DoesPostExist(request.PostId))
            {
                return NotFound("The post does not exist.");
            }

            if(!await _groupRepository.DoesGroupExist(request.GroupName))
            {
                return NotFound("The group does not exist.");
            }

            if(! await _userRepository.checkIfUserExists(userId))
            {
                return NotFound("The userName does not exist.");
            }

            var getLiked = await _likeRepository.FindLike(request);

            if(getLiked != null)
            {
                await _likeRepository.RemoveLikeToPost(getLiked);
                return NoContent();
            }

            var mappedRequest = _mapper.Map<Likes>(request);
            mappedRequest.UserId = userId;
            mappedRequest.LikedDate = DateTime.UtcNow;


            if (!await _likeRepository.AddLikeToPost(mappedRequest))
            {
                ModelState.AddModelError("","Couldn't like the post at the moment. Please try again later.");
                return BadRequest(ModelState);
            }

            return NoContent();
        }

    }
}
