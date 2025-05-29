using OlympiadApi.Models;
using System.Security.Claims;

namespace OlympiadApi.Helpers
{
    public interface IJwtHelper
    {
        string GenerateJwtToken(User user, Dictionary<string, Dictionary<string, bool>> rolesWithPermissions);
        bool ValidateJwtToken(string token);
        IEnumerable<Claim>? GetClaimsFromJwt(string token);
        Task<bool> ValidatePasswordAsync(int userId, string password);
    }
}
