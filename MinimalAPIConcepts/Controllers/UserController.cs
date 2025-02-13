using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;

namespace MinimalAPIConcepts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(500)]
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
                return StatusCode(500, ex);
            }
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);

                if (user == null) // Check if the user is null
                {
                    return NotFound("User not found!");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(user); // Return OK response with the user data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            try
            {
                if (user == null) return BadRequest(ModelState);

                var userAlreadyExists = await _userRepository.EmailExists(user.Email);
                if (userAlreadyExists)
                {
                    return BadRequest("User Already exists");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var newUser = await _userRepository.CreateUserAsync(user);
                if (!newUser)
                {
                    return BadRequest();
                }

                return StatusCode(201, "User successfully Created");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error " + ex);
            }
        }

        [HttpPut("{Id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updatedUser, Guid Id)
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
            var findUser = _userRepository.GetUserByIdAsync(Id);
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
