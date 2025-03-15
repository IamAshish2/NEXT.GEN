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

        public PostController(IpostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        [HttpGet("get-all-posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            try
            {
                var posts = await _postRepository.GetAllPosts();
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("get-user/{userId}")]
        public async Task<IActionResult> GetPostsByUser(int userId)

        {
            try
            {
                var posts = await _postRepository.GetPostsByUser(userId);
                if (posts == null || !posts.Any())
                    return NotFound($"No posts found for user with ID {userId}");

                return Ok(posts);
            }
            catch (Exception ex)
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

                if (!newPost.UserId.HasValue)
                    return BadRequest("The user id was not found.");

                var mappedPost = _mapper.Map<CreatePost>(newPost);
                mappedPost.PostedDate = DateTime.UtcNow;
                
                if (!await _postRepository.CreatePost(mappedPost))
                    return StatusCode(500, "An error occurred while creating the post.");

                return NoContent();
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
