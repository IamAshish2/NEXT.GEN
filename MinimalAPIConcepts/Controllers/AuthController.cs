using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Dtos.UserDto;
using NEXT.GEN.Models;
using System.Security.Claims;

namespace MinimalAPIConcepts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase    
    {
        
        private readonly ApplicationDbContext _context;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ApplicationDbContext context,ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _tokenGenerator = tokenGenerator;
            _jwtSettings = jwtSettings.Value;
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
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUser)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse { ErrorCode = "INVALID_INPUT", Message = "Invalid input data." });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginUser.Email && u.UserName == loginUser.UserName);

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

                // Set the token in an HTTP-only cookie
                //var cookieOptions = new CookieOptions
                //{
                //    HttpOnly = true,
                //    Secure = true,
                //    SameSite = SameSiteMode.Strict,
                //    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes))
                //};
                //Response.Cookies.Append("auth_token", token, cookieOptions);

                var response = new LoginResponseDto
                {
                    Token = token,
                    UserName = user.UserName
                };

                return Ok(response);
            }
            catch (Exception Error)
            {

                return BadRequest(Error.Message);
            }
        }


        [HttpGet("getUserByToken/{token}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public  IActionResult getUserNameFromToken(string token)
        {
            if (token == null) return BadRequest();

            var validToken =  _tokenGenerator.ValidateToken(token);

            if(validToken == null)
            {
                return BadRequest("The token is not valid");
            }

            var userName = validToken.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name);

            if(userName == null)
            {
                return BadRequest("The user name is null");
            }

            return Ok(new { userName});
        }
    }
}
