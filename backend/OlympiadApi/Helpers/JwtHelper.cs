using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OlympiadApi.Models;
using OlympiadApi.Services;
using Newtonsoft.Json;


namespace OlympiadApi.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;

        public JwtHelper(IConfiguration configuration, UserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        public string GenerateJwtToken(User user, Dictionary<string, Dictionary<string, bool>> rolesWithPermissions)
        {
            var secretKey = _configuration["JWT_SECRET_KEY"];
            var issuer = _configuration["JWT_ISSUER"];
            var audience = _configuration["JWT_AUDIENCE"];
            var tokenExpiryHours = _configuration["JWT_EXPIRATION_HOURS"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new InvalidOperationException("JWT_SECRET_KEY is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(issuer))
                throw new InvalidOperationException("JWT_ISSUER is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("JWT_AUDIENCE is not configured in the environment.");

            if (string.IsNullOrWhiteSpace(tokenExpiryHours) || !int.TryParse(tokenExpiryHours, out var expiryHours))
                throw new InvalidOperationException("JWT_EXPIRATION_HOURS is not configured correctly in the environment.");

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
                expires: DateTime.Now.AddHours(expiryHours),
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

        public bool ValidatePassword(int userId, string password)
        {
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public IEnumerable<Claim>? GetClaimsFromJwt(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken?.Claims;
        }

    }
}
