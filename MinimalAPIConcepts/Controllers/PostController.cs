using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/post
        [HttpGet]
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

        // GET: api/post/user/{userId}
        [HttpGet("user/{userId}")]
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

        // POST: api/post
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] Post newPost)
        {
            try
            {
                if (newPost == null)
                    return BadRequest("Post data is required.");

                var success = await _postRepository.CreatePost(newPost);
                if (!success)
                    return StatusCode(500, "An error occurred while creating the post.");

                return CreatedAtAction(nameof(GetAllPosts), new { id = newPost.PostId }, newPost);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // PUT: api/post/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] Post updatedPost)
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

        // DELETE: api/post/{id}
        [HttpDelete("{id}")]
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
