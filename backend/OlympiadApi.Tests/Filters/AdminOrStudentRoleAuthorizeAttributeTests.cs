using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using OlympiadApi.Filters;
using OlympiadApi.Helpers;

namespace OlympiadApi.Tests.Filters
{
    public class AdminOrStudentRoleAuthorizeAttributeTests
    {
        private AuthorizationFilterContext CreateContextWithAuthHeader(string token)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = token;

            var actionContext = new ActionContext
            {
                HttpContext = httpContext,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor()
            };

            var filters = new List<IFilterMetadata>();
            var context = new AuthorizationFilterContext(actionContext, filters);
            return context;
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenTokenIsMissing()
        {
            var jwtHelper = new Mock<IJwtHelper>();

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenTokenIsInvalid()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("invalid-token")).Returns(false);

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer invalid-token");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenNoClaims()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>());

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsUnauthorized_WhenClaimsIsNull()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns((IEnumerable<Claim>?)null);

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<UnauthorizedObjectResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsForbidden_WhenRolesClaimIsMissingOrEmpty()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("Other", "Data")
        });

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<ForbidResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ReturnsForbidden_WhenRolesDoNotMatch()
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("Roles", "User")
        });

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.IsType<ForbidResult>(context.Result);
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("Student")]
        [InlineData("Admin,Student")]
        public void OnAuthorization_AllowsAccess_WhenRoleIsValid(string roleValue)
        {
            var jwtHelper = new Mock<IJwtHelper>();
            jwtHelper.Setup(j => j.ValidateJwtToken("valid")).Returns(true);
            jwtHelper.Setup(j => j.GetClaimsFromJwt("valid")).Returns(new List<Claim>
        {
            new Claim("Roles", roleValue)
        });

            var attribute = new AdminOrStudentRoleAuthorizeAttribute(jwtHelper.Object);
            var context = CreateContextWithAuthHeader("Bearer valid");

            attribute.OnAuthorization(context);

            Assert.Null(context.Result);
        }
    }
}