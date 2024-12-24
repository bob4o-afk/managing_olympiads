using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OlympiadApi.Models;

namespace OlympiadApi.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;

        public JwtHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {
            var secretKey = _configuration["JWT_SECRET_KEY"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT_ISSUER is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT_AUDIENCE is not configured in the environment.");

            // Generate the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
