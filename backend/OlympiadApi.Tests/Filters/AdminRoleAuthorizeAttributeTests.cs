using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using OlympiadApi.Filters;
using OlympiadApi.Helpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace OlympiadApi.Tests.Filters
{
    public class AdminRoleAuthorizeAttributeTests
    {
        private AuthorizationFilterContext CreateContextWithAuthHeader(string token)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = token;

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor()
            };

            var filters = new List<IFilterMetadata>();
            return new AuthorizationFilterContext(actionContext, filters);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenTokenIsMissing()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("invalid")).Returns(false);

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer invalid");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenNoClaims()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>());

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenClaimsAreNull()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns((IEnumerable<Claim>?)null);

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsForbidden_WhenRolesClaimIsMissing()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("SomeOtherClaim", "value")
        });

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<ForbidResult>(context.Result);
        }


        [Fact]
        public void OnAuthorization_ReturnsForbidden_WhenNotAdmin()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("Roles", "Student")
        });

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<ForbidResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_AllowsAccess_WhenAdminRoleIsPresent()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("Roles", "Admin")
        });

            var attribute = new AdminRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.Null(context.Result);
        }
    }
}