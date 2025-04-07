using Microsoft.AspNetCore.Mvc;
using NEXT.GEN.Dtos.Otp;
using NEXT.GEN.Services.Interfaces;
using AutoMapper;
using NEXT.GEN.ExceptionHandlers;
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

            string emailBody = $@"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <title>Verify Your Email Address</title>
                                    <style>
                                        body {{
                                            font-family: sans-serif;
                                            line-height: 1.6;
                                            color: #333;
                                            background-color: #f4f4f4;
                                            margin: 0;
                                            padding: 0;
                                        }}
                                        .container {{
                                            max-width: 600px;
                                            margin: 20px auto;
                                            background-color: #fff;
                                            padding: 30px;
                                            border-radius: 8px;
                                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                        }}
                                        h1 {{
                                            color: #007bff;
                                            margin-bottom: 20px;
                                            text-align: center;
                                        }}
                                        p {{
                                            margin-bottom: 15px;
                                        }}
                                        .otp-container {{
                                            background-color: #e9ecef;
                                            padding: 15px;
                                            border-radius: 6px;
                                            text-align: center;
                                            margin-bottom: 20px;
                                        }}
                                        .otp {{
                                            font-size: 24px;
                                            font-weight: bold;
                                            color: #28a745;
                                            letter-spacing: 10px;
                                        }}
                                        .button {{
                                            display: inline-block;
                                            background-color: #007bff;
                                            color: #fff;
                                            padding: 10px 20px;
                                            text-decoration: none;
                                            border-radius: 5px;
                                        }}
                                        .button:hover {{
                                            background-color: #0056b3;
                                        }}
                                        .disclaimer {{
                                            font-size: 0.8em;
                                            color: #777;
                                            margin-top: 20px;
                                        }}
                                        .footer {{
                                            margin-top: 30px;
                                            text-align: center;
                                            color: #777;
                                            font-size: 0.9em;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class=""container"">
                                        <h1>Verify Your Email Address</h1>
                                        <p>Hello,</p>
                                        <p>Thank you for signing up or attempting to access a feature that requires email verification. To complete the process, please use the One-Time Password (OTP) provided below:</p>

                                        <div class=""otp-container"">
                                            <span class=""otp"">{otp}</span>
                                        </div>

                                        <p>Please enter this OTP on the verification page. This OTP is valid for a limited time.</p>

                                        <p>If you did not request this verification, you can safely ignore this email. No action is required on your part.</p>

                                        <p>Thank you,</p>
                                        <p>The NEXT.GEN Team</p>

                                        <div class=""disclaimer"">
                                            <p>This is an automatically generated email. Please do not reply to this message.</p>
                                        </div>

                                        <div class=""footer"">
                                            &copy; [{DateTime.UtcNow.Year}] [NEXT.GEN]. All rights reserved.
                                        </div>
                                    </div>
                                </body>
                                </html>";

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
                // return StatusCode(500, new ErrorResponse { ErrorCode = "INTERNAL_SERVER_ERROR", Message = "An unexpected error occurred." });
                    return StatusCode(500, "An unexpected error occurred");
            }
        }
    }
}
