using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;

namespace MinimalAPIConcepts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<User> _logger;

        public UserController(IUserRepository userRepository, IMapper mapper, ILogger<User> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500,Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                if (users == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid) return BadRequest(ModelState);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { ErrorCode = "ERROR",Message="Server Error! Please try again later."});
            }
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(GetUserDto))] 
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))] 
        public async Task<ActionResult<GetUserDto>> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ErrorResponse { ErrorCode = "USER_NOT_FOUND", Message = "User not found." }); 
                }

                var mappedUser = _mapper.Map<GetUserDto>(user);

                return Ok(mappedUser);

            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error retrieving user: {userId}", userId); 
                return StatusCode(500, new ErrorResponse { ErrorCode = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." });
            }
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
        {
            try
            {
                if (user == null) return BadRequest(ModelState);

                var emailInUse = await _userRepository.isEmailInUse(user.Email);
                var userNameInUse = await _userRepository.isUserNameInUse(user.UserName);
                if (emailInUse || userNameInUse)
                {
                    return BadRequest("User Already exists");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _logger.LogInformation("The users hashed password is " , hashedPassword);
                user.Password = hashedPassword;

                var mappedUser = _mapper.Map<User>(user);

                var newUser = await _userRepository.CreateUserAsync(mappedUser);
                if (!newUser)
                {
                    return BadRequest();
                }

                return StatusCode(201, "User successfully Created");

            }
            catch (Exception ex)
            {
                _logger.LogInformation("Error occured while creating a new user. Error is " + ex.Message);
                return StatusCode(500, new ErrorResponse { ErrorCode = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." });
            }
        }

        [HttpPut("{Id}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser(Guid Id, [FromBody] UpdateUserDto updatedUser)
        {
            if(!ModelState.IsValid)  return BadRequest(ModelState);

            var userExists = await _userRepository.checkIfUserExists(Id);
            if (!userExists){
                return NotFound();
            }

            var userMap = _mapper.Map<User>(updatedUser);

            bool success = await _userRepository.UpdateUserAsync(Id,userMap);
            if (!success)
            {
                ModelState.AddModelError("", "Unsuccessful operation. Try again!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("deleteUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        
        public async Task<IActionResult> DeleteUser(Guid Id)
        {
            var findUser = await _userRepository.GetUserByIdAsync(Id);
            if (findUser == null)
            {
                return NotFound();
            }

            bool success = await _userRepository.DeleteUserAsync(Id);
            if (!success)
            {
                ModelState.AddModelError("","An error occured while deleting the user");
                return StatusCode(500, ModelState); 
            }

            return Ok("Success deletion");
        }


    }
}
