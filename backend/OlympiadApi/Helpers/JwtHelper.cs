using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OlympiadApi.Models;
using OlympiadApi.Services.Interfaces;
using Newtonsoft.Json;

namespace OlympiadApi.Helpers
{
    public class JwtHelper : IJwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public JwtHelper(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        public string GenerateJwtToken(User user, Dictionary<string, Dictionary<string, bool>> rolesWithPermissions)
        {
            var secretKey = _configuration["JWT_SECRET_KEY"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];
            var tokenExpiryMinutes = _configuration["JWT_EXPIRATION_MINUTES"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT_ISSUER is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT_AUDIENCE is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(tokenExpiryMinutes) || !int.TryParse(tokenExpiryMinutes, out var expiryMinutes))
                throw new InvalidOperationException("JWT_EXPIRATION_MINUTES is not configured correctly in the environment.");

            // Generate the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("Roles", JsonConvert.SerializeObject(rolesWithPermissions))
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateJwtToken(string token)
        {
            var secretKey = _configuration["JWT_SECRET_KEY"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY is not configured in the environment.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public IEnumerable<Claim>? GetClaimsFromJwt(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
    }
}
