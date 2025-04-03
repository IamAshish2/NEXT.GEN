using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.PostDto;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IpostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly ILogger<CreatePost> _logger;

        public PostController(
            IpostRepository postRepository,
            IMapper mapper,IGroupRepository groupRepository,
            IGroupMemberRepository groupMemberRepository,
            ILogger<CreatePost> logger
            )
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _groupRepository = groupRepository;
            _groupMemberRepository = groupMemberRepository;
            _logger = logger;
        }



    // i need to return all the posts made to a group
    // returns all the posts made in a group 
        [HttpGet("get-all-group-posts/{groupName}")]
        public async Task<ActionResult<ICollection<GetGroupPostsDto>>> GetAllPosts(string groupName)
        {
            Request.Cookies.TryGetValue("userId", out var userId);
            if (userId == null)
            {
                return BadRequest("the username was not found");
            }

            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest("Group name is required.");
            }

            if (! await _groupMemberRepository.GroupExists(groupName)){
                return NotFound(); 
            }

            try
            {
                var posts = await _postRepository.GetAllPostsFromGroup(groupName,userId);

                // if there are no posts in the group then, return an empty list
                if (posts == null || posts.Count == 0)
                {
                    return Ok(new List<GetGroupPostsDto>());
                }

                return Ok(posts);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"Argument exception occurred while getting posts for group: {groupName}");
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"Invalid operation exception occurred while getting posts for group: {groupName}");
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while getting posts for group: {groupName}");
                return StatusCode(500, "An internal server error occurred.");
            }
        }


        [HttpGet("get-post-details-by-id/{postId}")]
        [ProducesResponseType(401)]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<GetGroupPostsDto>> GetPostDetailsById(int postId)
        {
            // check if the post exists
            
            if(! await _postRepository.DoesPostExist(postId))
            {
                return BadRequest("The post was not found.");
            }

            var postDetails = await _postRepository.GetPostDetailsById(postId);
            if(postDetails == null)
            {
                return Ok(new GetGroupPostsDto { });
            }

            return Ok(postDetails);
        }

        [HttpGet("get-user/{userName}")]
        public async Task<IActionResult> GetPostsByUser(string userName)

        {
            try
            {
                var posts = await _postRepository.GetPostsByUser(userName);
                if (posts == null || !posts.Any())
                    return NotFound($"No posts found for user with ID {userName}");

                return Ok(posts);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostDto newPost)
        {
            try
            {
                if (newPost == null)
                    return BadRequest("Post data is required.");

                if (newPost.userName == null)
                    return BadRequest("The user id was not found.");

                var mappedPost = _mapper.Map<CreatePost>(newPost);
                mappedPost.PostedDate = DateTime.UtcNow;
                _logger.LogInformation("",mappedPost.PostedDate);
                
                if (!await _postRepository.CreatePost(mappedPost))
                    return StatusCode(500, "An error occurred while creating the post.");

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPut("update-post/{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] CreatePost updatedPost)
        {
            try
            {
                if (updatedPost == null || id != updatedPost.PostId)
                    return BadRequest("Invalid post data.");

                var success = await _postRepository.UpdatePost(updatedPost);
                if (!success)
                    return StatusCode(500, "An error occurred while updating the post.");

                return Ok("Post updated successfully.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            try
            {
                var posts = await _postRepository.GetAllPosts();
                var postToDelete = posts.FirstOrDefault(p => p.PostId == id);

                if (postToDelete == null)
                    return NotFound($"Post with ID {id} not found.");

                var success = await _postRepository.DeletePost(postToDelete);
                if (!success)
                    return StatusCode(500, "An error occurred while deleting the post.");

                return Ok("Post deleted successfully.");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
