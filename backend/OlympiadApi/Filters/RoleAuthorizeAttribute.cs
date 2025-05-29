using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OlympiadApi.Helpers;

namespace OlympiadApi.Filters
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtHelper = context.HttpContext.RequestServices.GetService<IJwtHelper>();
            if (jwtHelper == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is missing or invalid." });
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();
            if (!jwtHelper.ValidateJwtToken(token))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is invalid or expired." });
                return;
            }

            var claims = jwtHelper.GetClaimsFromJwt(token);
            var roles = claims?.FirstOrDefault(c => c.Type == "Roles")?.Value;

            if (string.IsNullOrEmpty(roles) || !_allowedRoles.Any(roles.Contains))
            {
                context.Result = new ForbidResult("You are not authorized to perform this action.");
            }
        }
    }
}