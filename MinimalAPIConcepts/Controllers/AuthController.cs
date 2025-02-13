using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIConcepts.Context;
using MinimalAPIConcepts.Dtos.UserDto;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
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

        [HttpPost]
        public IActionResult Login([FromBody] LoginUserDto newUser)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var user = _context.Users
                      .FirstOrDefault(u => u.Email == newUser.Email && u.Password == newUser.Password && u.UserName == newUser.UserName);

            if (user == null)
            {
                return Unauthorized();
            }

            var token = _tokenGenerator.GenerateToken(newUser.UserName,newUser.Email);
            return Ok(new { token });
        }


      
    }
}
