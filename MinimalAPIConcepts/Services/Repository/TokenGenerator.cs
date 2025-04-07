using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NEXT.GEN.Context;
using NEXT.GEN.Models;
using NEXT.GEN.Services.Interfaces;

namespace NEXT.GEN.Services.Repository
{
    public class TokenGenerator: ITokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;

        // Using the Ioptions interface to leverage the Configure builder Service
        public TokenGenerator(IOptions<JwtSettings> jwtSettings, ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }
        public string GenerateToken(User user)
        {
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim("email",user.Email)
            };

            //  the custom key that we have set in the appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            //create signingCredentials and hash it with a algorithm.
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            
            // Create a instance of JWTSecurityToken() for creating a token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_jwtSettings.ExpireMinutes)),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateRefreshToken()
        {
            // get random number from 64 bytes array
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var alreadyInUse = await CheckIfRefreshTokenExist(refreshToken);

            if(alreadyInUse)
            {
                return await GenerateRefreshToken();
            }

            return refreshToken;
        }


        public async Task<bool> CheckIfRefreshTokenExist(string token)
        {
            return await _context.RefreshTokens.AnyAsync(r => r.Token == token);
        }

        public  async Task InsertRefreshToken(string userName, string refreshToken)
        {
            var newRefreshToken = new RefreshToken 
            { 
                userName = userName,
                Token   = refreshToken,
                ExpirationTime = DateTime.Now.AddDays(7)
            };

            await _context.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();
        }

        /*public ClaimsPrincipal ValidateToken(string token)
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
            */
    }
}
