namespace NEXT.GEN.Services.HTTPONLY_MIDDLEWARE
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using MinimalAPIConcepts.Models;

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("auth_token", out var token))
            {
                var principal = ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                }
            }

            await _next(context);
        }

        private ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.ASCII.GetBytes(_jwtSettings.Key);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
