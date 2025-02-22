using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPIConcepts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase    
    {
        
        private readonly ApplicationDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthController(ApplicationDbContext context,ITokenGenerator tokenGenerator)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
        }

        [HttpGet]
        [Authorize]
        // This endpoint can only be hit by the authorized user i.e. admin in this case. 
        // so, if the endpoint returns 200 ok , the user is authorized
        // else the user is not authorized.
        public IActionResult verifyToken()
        {
            return Ok(new { message = "user Authorized" });
        }


        [HttpPost("login")] 
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))] 
        [ProducesResponseType(401, Type = typeof(ErrorResponse))] 
        public IActionResult Login([FromBody] LoginUserDto loginUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { ErrorCode = "INVALID_INPUT", Message = "Invalid input data." });
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == loginUser.Email && u.UserName == loginUser.UserName);

            if (user == null)
            {
                return Unauthorized(new ErrorResponse { ErrorCode = "USER_NOT_FOUND", Message = "User not found." });
            }

            var checkPassword = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password); 

            if (!checkPassword)
            {
                return Unauthorized(new ErrorResponse { ErrorCode = "INVALID_CREDENTIALS", Message = "Invalid credentials." });
            }

            var token = _tokenGenerator.GenerateToken(loginUser.UserName, loginUser.Email);

            var response = new LoginResponseDto
            {
                Token = token,
                Id= user.Id,
                UserName = user.UserName
            };

            return Ok(response);
        }
    }
}
