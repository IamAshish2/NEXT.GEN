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
                new Claim(JwtRegisteredClaimNames.Sub, userName),
            };

            //  the custom key that we have set in the appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            //create signingCredentials and hash it with a algorithm.
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

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if(validatedToken is JwtSecurityToken)
                {
                    return principal;
                }

                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
            

        }
    }
}
