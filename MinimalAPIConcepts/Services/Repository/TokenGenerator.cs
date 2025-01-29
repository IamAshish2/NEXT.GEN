using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MinimalAPIConcepts.Models;
using MinimalAPIConcepts.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPIConcepts.Services.Repository
{
    public class TokenGenerator: ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        // Using the Ioptions interface to leverage the Configure builder Service
        public TokenGenerator(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string GenerateToken(string userName, string Email)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,Email),
                new Claim(ClaimTypes.Name,userName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes)),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
