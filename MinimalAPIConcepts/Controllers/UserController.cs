using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinimalAPIConcepts.Dtos.UserDto;
using NEXT.GEN.Dtos.Otp;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.ExceptionHandlers;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

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

        [HttpGet("get-all-users")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<GetUserDto>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                if (users == null)
                {
                    return NotFound();
                }
                var mappedUsers = _mapper.Map<List<GetUserDto>>(users);
                return Ok(mappedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { ErrorCode = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet("get-user-by-name/")]
        [ProducesResponseType(200, Type = typeof(GetUserDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500, Type = typeof(ErrorResponse))]
        public async Task<ActionResult<GetUserDto>> GetUserById()
        {
            try
            {
                Request.Cookies.TryGetValue("userId",out var userId);
                if (String.IsNullOrEmpty(userId))
                {
                    return BadRequest(new ErrorResponse{ Message="The username was not found.",ErrorCode="404" });
                }
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
                _logger.LogError(ex, "Error retrieving user");
                return StatusCode(500, new ErrorResponse { ErrorCode = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." });
            }
        }


        [HttpPost("create-user")]
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
                _logger.LogInformation("The users hashed password is ", hashedPassword);
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

        [HttpPut("update-user/")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updatedUser)
        {
            Request.Cookies.TryGetValue("userId",out var userId);
            Request.Cookies.TryGetValue("userName",out var userName);
            if (String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(userName))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (userId == null) return BadRequest();

            //if (userName.Trim() != updatedUser.UserName.Trim()) return BadRequest();

            //var userExists = await _userRepository.checkIfUserExists(Id);
            var userExists = await _userRepository.checkIfUserExists(userId);
            if (!userExists)
            {
                return NotFound();
            }

            var existingUser = await _userRepository.GetUserByNameAsync(userName);

            if(existingUser == null)
            {
                return NotFound();
            }

            // Map UpdateUserDto to GetUserDto while ignoring null values
            //var mappedDto = _mapper.Map(updatedUser, existingUser);

            // Map GetUserDto to User entity while ignoring null values
            _mapper.Map(updatedUser, existingUser);


            bool success = await _userRepository.UpdateUserAsync(existingUser);
            if (!success)
            {
                ModelState.AddModelError("", "Unsuccessful operation. Try again!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("deleteUser/{userName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            var findUser = await _userRepository.GetUserByNameAsync(userName);
            if (findUser == null)
            {
                return NotFound();
            }

            bool success = await _userRepository.DeleteUserAsync(userName);
            if (!success)
            {
                ModelState.AddModelError("", "An error occured while deleting the user");
                return StatusCode(500, ModelState);
            }

            return Ok("Success deletion");
        }

        // only the users who have a valid accesstoken can hit this api
        [HttpGet("get-user-profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        //[Authorize]
        public async Task<ActionResult<GetUserDto>> GetUserProfile()
        {
            /* watch how to extract the data embedded in token 
            // extract the name from the jwt token
            //var userName = User.FindFirstValue(ClaimTypes.Name);
            */

            // we extracted the username from the cookies data successfully
             Request.Cookies.TryGetValue("userId",out var userId);
            if (userId == null)
            {
                return BadRequest("the username was not found");
            }

            var user = await _userRepository.GetUserByNameAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var mappedUser = _mapper.Map<GetUserDto>(user);

            return Ok(mappedUser);
        }

}
}
