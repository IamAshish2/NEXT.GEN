using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Dtos.CommentDto;
using NEXT.GEN.Models.PostModel;
using NEXT.GEN.Services.Interfaces;
using System;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(IMapper mapper, ICommentRepository commentRepository, IpostRepository postRepository,
        IUserRepository userRepository) : ControllerBase
    {
        private readonly IMapper _mapper = mapper;
        private readonly ICommentRepository _commentRepository = commentRepository;
        private readonly IpostRepository _postRepository = postRepository;
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost("comment")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto comment)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Request.Cookies.TryGetValue("userId", out var userId);

            if (String.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }

            if (!await _postRepository.DoesPostExist(comment.PostId))
            {
                return NotFound("The post does not exist.");
            }

            if (!await _userRepository.checkIfUserExists(userId))
            {
                return NotFound("The userName does not exist.");
            }

            var mappedComment = _mapper.Map<Comment>(comment);
            mappedComment.UserId = userId;
            mappedComment.CommentDate = DateTime.Now;
            if (!await _commentRepository.CreateComment(mappedComment))
            {
                ModelState.AddModelError("", "The comment could not be created at the moment.");
                return BadRequest(ModelState);
            }

            return NoContent();
        }



        [HttpGet("get-comments-by-post/{postId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCommentsByPost(int postId)
        {
            try
            {
                if (!await _postRepository.DoesPostExist(postId))
                {
                    return NotFound("The post does not exist.");
                }

                var postComments = await _commentRepository.GetPostComments(postId);
                if (postComments == null)
                {
                    return Ok(new GetPostCommentsDto { });
                }

                return Ok(postComments);
            } catch(System.Exception ex)
            {
                return BadRequest(ModelState);
            }
        }
    }
}
