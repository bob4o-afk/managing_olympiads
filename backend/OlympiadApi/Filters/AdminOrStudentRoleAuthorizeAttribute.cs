using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OlympiadApi.Helpers;

namespace OlympiadApi.Filters
{
    public class AdminOrStudentRoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IJwtHelper _jwtHelper;

        public AdminOrStudentRoleAuthorizeAttribute(IJwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is missing or invalid." });
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            if(!_jwtHelper.ValidateJwtToken(token)){
                context.Result = new UnauthorizedObjectResult(new { message = "Token is invalid or expired." });
            }
            var claims = _jwtHelper.GetClaimsFromJwt(token);

            if (claims == null || !claims.Any())
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token is invalid or expired." });
                return;
            }

            var roles = claims.FirstOrDefault(c => c.Type == "Roles")?.Value;
            if (string.IsNullOrEmpty(roles) || (!roles.Contains("Admin") && !roles.Contains("Student")))
            {
                context.Result = new ForbidResult("You are not authorized to perform this action.");
                return;
            }
        }
    }
}
