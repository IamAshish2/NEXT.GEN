using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.Otp;
using NEXT.GEN.Services.Interfaces;
using AutoMapper;
using NEXT.GEN.ExceptionHandlers;
using NEXT.GEN.global.EmailTemplates;
using User = NEXT.GEN.Models.User;

namespace NEXT.GEN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase 
    {
        private readonly IOtpRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public OtpController(IOtpRepository otpRepository, IEmailService emailService, IUserRepository userRepository, IMapper mapper)
        {
            _otpRepository = otpRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost("request-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> GenerateOtp(CreateUserDto request)
        {
            var otp = await _otpRepository.GenerateRandomOtp();
            if (String.IsNullOrWhiteSpace(otp))
            {
                return BadRequest();
            }
            // check if a user already exists with similar email
            var emailInUse = await _userRepository.isEmailInUse(request.Email);
            var userNameInUse = await _userRepository.isUserNameInUse(request.UserName);
            if (emailInUse || userNameInUse)
            {
                return BadRequest("User Already exists");
            }

            // save the otp to the memory cache
            await _otpRepository.storeOtp(request.Email, otp, request);

            // get the email template for email verification
            string emailBody = EmailTemplates.GetVerificationForSignupEmailTemplate(otp);
            // send the email with the formatted HTML body
            if (! await _emailService.sendEmail(request.Email, emailBody, "Verify your email"))
            {
               await  _otpRepository.deleteOtp(request.Email);
                return BadRequest();
            }

            return NoContent(); 
        }

        [HttpPost("verify-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult> VerfiyOtp([FromBody] VerifyOtpRequestDto request)
        {
            try
            {

                // Validate request
                if (request == null || string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Invalid request");
                }

                // get the email from the cache | check if the email exists in current context
                if (String.IsNullOrWhiteSpace(request.Email))
                {
                    return Unauthorized("Please resubmit the form!");
                }
                
                // get the stored otp and check with the request
                var storedOtp =  await _otpRepository.getStoredOtpAsync(request.Email);
                if (request.Code.Trim() != storedOtp )
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorCode = "OTP_NOT_FOUND",
                        Message = "OTP expired or not found. Please request a new one."
                    });
                }
                
                // if the otp matches, then create a new user
                var storedUser = await _otpRepository.getStoredUserDto(request.Email);
                
                // create a new user
                    if (storedUser == null) return BadRequest(ModelState);

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(storedUser.Password);
                    storedUser.Password = hashedPassword;

                    var mappedUser = _mapper.Map<User>(storedUser);

                    var newUser = await _userRepository.CreateUserAsync(mappedUser);
                    if (!newUser)
                    {
                        return BadRequest();
                    }

                    return StatusCode(201, "User successfully Created");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { ErrorCode = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." });
                    // return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
