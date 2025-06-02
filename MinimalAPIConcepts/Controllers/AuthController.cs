namespace NEXT.GEN.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MinimalAPIConcepts.Dtos.UserDto;
    using NEXT.GEN.Context;
    using NEXT.GEN.Dtos.Otp;
    using NEXT.GEN.ExceptionHandlers;
    using NEXT.GEN.global;
    using Models.UserModel;
    using NEXT.GEN.Services.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            ApplicationDbContext context, IMapper mapper ,IHttpContextAccessor  httpContextAccessor ,
            ITokenGenerator tokenGenerator, IOptions<JwtSettings> jwtSettings, IUserRepository userRepository,
            IOtpRepository otpRepository, IEmailService emailService, IWebHostEnvironment environment,
            ILogger<AuthController> logger
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _tokenGenerator = tokenGenerator;
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _emailService = emailService;
            _environment = environment;
            _logger = logger;
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
                    return Unauthorized(new ErrorResponse { ErrorCode = "USER_NOT_FOUND", Message = "The username or email does not exist." });
                }

                var checkPassword = BCrypt.Net.BCrypt.Verify(loginUser.Password, user.Password);

                if (!checkPassword)
                {
                    return Unauthorized(new ErrorResponse { ErrorCode = "INVALID_CREDENTIALS", Message = "Enter the correct details to login." });
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

        [HttpPost("forgot-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            try
            {
                // check if the email exists
                if (!await _userRepository.isEmailInUse(email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Sorry, The email does not exist.",
                        ErrorCode = "EMAIL_NOT_FOUND"
                    });
                }
                
                // if the email does exist, then send a otp to the user's email
                var code = await  _otpRepository.GenerateRandomOtp();
                
                // save the otp with the email in the cache 
                // now the user will get the code and when the user enters the code before expiry time,
                // we can check it from the cache and verify the code to let the uesr reset the password
                // before sending the generated code to the user, save the code along with the user's email in the cache memory
                // await _otpRepository.storeOtpForResetingPassword(email,code);
                var normalizedEmail = email.Trim().ToLower();
                await _otpRepository.storeOtpForResetingPassword(normalizedEmail,code);

                _logger.LogInformation("OTP stored for email: {Email}, OTP: {OTP}", email, code);

                
                // trying to read the html file from global folder
                var path = Path.Combine(_environment.ContentRootPath,"global","EmailTemplates","ForgotPasswordCodeRequest.html");
                var emailBody = await System.IO.File.ReadAllTextAsync(path);
                emailBody = emailBody.Replace("{code}",code);
                emailBody = emailBody.Replace("{DateTime.UtcNow.Year}",DateTime.UtcNow.Year.ToString());
                
              if (! await _emailService.sendEmail(email,emailBody,"Password Reset Code"))
              {
                  return BadRequest(new ErrorResponse
                  {
                      Message = "Please try again later. The service is not available at the moment.",
                      ErrorCode = "EMAIL_NOT_SENT"
                  });
              }

              return Ok(new SuccessResponse
              {
                  Message = "Please check your email for verification code.",
                  Severity = "success"
              });
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Please try again later. The request couldn't be fulfilled right now.",
                    ErrorCode = "Failed"
                });
            }
        }


        [HttpPost("validate-forgot-password-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> ValidateForgotPasswordOtp(ValidateForgotPasswordOtpRequestDto request)
        {
            try
            {
                _logger.LogInformation("The requested email is" + request.Email);
                // var storedCode = await _otpRepository.getStoredOtpForResettingPassword(request.Email);
                var normalizedEmail = request.Email.Trim().ToLower();
                var storedCode = await _otpRepository.getStoredOtpForResettingPassword(normalizedEmail);
                _logger.LogInformation("The stored code is " + storedCode);
                _logger.LogInformation("Stored OTP: {Stored}, Provided OTP: {Provided}", storedCode, request.Code);

                if (string.IsNullOrWhiteSpace(storedCode) || string.IsNullOrWhiteSpace(request.Code) ||
                    !string.Equals(storedCode.Trim(), request.Code.Trim(), StringComparison.Ordinal))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Sorry, the code did not match. Check again and try. laude",
                        ErrorCode = "NO_MATCH"
                    });
                }

                return Ok(new SuccessResponse
                {
                    Message = "You can now Reset your password.",
                    Severity = "success"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Try again later.",
                    ErrorCode = "Server error"
                });
            }
        }

        [HttpPatch("reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            try
            {
                // check if the email exists
                if (!await _userRepository.isEmailInUse(request.Email))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Sorry, The email does not exist.",
                        ErrorCode = "NONE"
                    });
                }
                
                // if the email does exist, then patch the new password
                // get the user
                var user = await _userRepository.GetUserByEmailAsync(request.Email);

                // hash the password with bcrypt
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.newPassword);
                user.Password = hashedPassword;

                // invalidate the existing tokens for security
                user.SecurityStamp = new Guid().ToString();
                
                // now save the changes to the database
                if (!await _userRepository.SaveAsync())
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Password Reset Failed. Try again later.",
                        ErrorCode = "Server error"
                    });
                }
                
                // TODO ->  if the password is reset successfully then send an email to the user
                

                return Ok(new SuccessResponse
                {
                    Message = "Password reset successfully.",
                    Severity = "success"
                });

            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse
                {
                    Message = "Try again later.",
                    ErrorCode = "Server error"
                });
            }
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

            var user = await _userRepository.LoginWithGoogle(result.Principal);
            
            
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
            Response.Cookies.Append("userName",user.UserName,cookieOptions);

            return Redirect(returnUrl);
        }
    }
}
