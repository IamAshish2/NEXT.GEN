using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using NEXT.GEN.Exception;

namespace MinimalAPIConcepts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase    
    {
        
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ApplicationDbContext context, IMapper mapper ,IHttpContextAccessor  httpContextAccessor ,ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings, IUserRepository userRepository)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpGet]
        //[Authorize]
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
                    return Unauthorized(new ErrorResponse { ErrorCode = "USER_NOT_FOUND", Message = "The username or email does not exist." });
                }

                var checkPassword = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

                if (!checkPassword)
                {
                    return Unauthorized(new ErrorResponse { ErrorCode = "INVALID_CREDENTIALS", Message = "Invalid credentials." });
                }

                var token = _tokenGenerator.GenerateToken(user);
                var refreshToken = await _tokenGenerator.GenerateRefreshToken();
                await _tokenGenerator.InsertRefreshToken(loginUser.UserName,refreshToken);

                var cookieOptions = new CookieOptions
                {

                    // Set the token in an HTTP-only cookie
                    HttpOnly = true,
                    IsEssential = true,
                    // ensure communication over  https only
                    Secure = true,
                    // define samesitemode.none to persist the cookies on refresh and on application reload for frontend side
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes))
                };
                Response.Cookies.Append("auth_token", token, cookieOptions);
                Response.Cookies.Append("userName",user.UserName,cookieOptions);
                Response.Cookies.Append("userId", user.Id,cookieOptions);
                Response.Cookies.Append("refresh_token",refreshToken,cookieOptions);

                // send other data that you want 
                return Ok(new
                {
                    message = "Login successful",
                    userName = loginUser.UserName
                });
            }
            catch (Exception Error)
            {

                return BadRequest(Error.Message);
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400, Type = typeof(ErrorResponse))]
        [ProducesResponseType(401, Type = typeof(ErrorResponse))]
        public async Task<ActionResult> RequestRefreshToken()
        {
            if(!Request.Cookies.TryGetValue("auth_token",out var authToken) || !Request.Cookies.TryGetValue("userName",out var userName) || !Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            {
                return BadRequest();
            }

            var user = await _userRepository.GetUserByNameAsync(userName);

            if(user == null)
            {
                return BadRequest();
            }

            var cookieOptions = new CookieOptions
            {
                // Set the token in an HTTP-only cookie
                HttpOnly = true,
                IsEssential = true,
                // ensure communication over  https only
                Secure = true,
                // define samesitemode.none to persist the cookies on refresh and on application reload for frontend side
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes))
            };

            var token = _tokenGenerator.GenerateToken(user);
            var refresh = await _tokenGenerator.GenerateRefreshToken();
            await _tokenGenerator.InsertRefreshToken(user.UserName,refresh);

            Response.Cookies.Append("auth_token",token,cookieOptions);
            Response.Cookies.Append("refresh_token",refresh,cookieOptions);
            Response.Cookies.Append("userId", user.Id, cookieOptions);
            Response.Cookies.Append("userName",userName,cookieOptions);

            return Ok();

        }


        // this method should be more concise and a way to actually check if the currently logged in 
        // user is authenticated or not. make it more better.
        [HttpGet("check-auth")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        public IActionResult CheckAuth()
        {
            Request.Cookies.TryGetValue("auth_token",out var accessToken);
            if (String.IsNullOrEmpty(accessToken))
            {
                // the user is not authenticated
                return Unauthorized();
            }

            var principal = _userRepository.validateJWT(accessToken);
            if(principal == null)
            {
                return Unauthorized();
            }

            Request.Cookies.TryGetValue("userId",out var userId);
            Request.Cookies.TryGetValue("refresh_token",out var refreshToken);

            if(String.IsNullOrEmpty(userId) || String.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ErrorResponse { ErrorCode = "401", Message = "You have been logged out. Please login again."});
            }

            // ok response means that the token is verified and is okay
            return Ok(new {isLogggedIn = true});
        }


        [HttpGet("/api/account/login/google")]
        public IActionResult SignUpWithGoogle([FromQuery] string returnUrl, LinkGenerator linkGenerator, SignInManager<User> signInManager)
        {
            //var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", linkGenerator.GetPathByName(_httpContextAccessor.HttpContext,
            //    "/api/account/login/google/callback") + $"?returnUrl={returnUrl}");
            // the EscapeDataString function is used to encode the returnUrl
            // google does not allow the unencoded url to proecess and will throw an error
            var callbackUrl = "/api/account/login/google/callback?returnUrl=" + Uri.EscapeDataString(returnUrl);
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

            return Challenge(properties, ["Google"]);
        }

        [HttpGet("/api/account/login/google/callback")]
        public async Task<IActionResult> GoogleSignUpCallback([FromQuery] string returnUrl)

        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            await _userRepository.LoginWithGoogle(result.Principal);

            return Redirect(returnUrl);
        }
    }
}
