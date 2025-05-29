using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using OlympiadApi.Filters;
using OlympiadApi.Helpers;

namespace OlympiadApi.Tests.Filters
{
    public class RoleAuthorizeAttributeTests
    {
        private AuthorizationFilterContext CreateContext(string authHeader, IJwtHelper jwtHelper)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(x => x.GetService(typeof(IJwtHelper))).Returns(jwtHelper);

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = serviceProvider.Object;
            if (authHeader != null)
                httpContext.Request.Headers["Authorization"] = authHeader;

            var actionContext = new ActionContext(
                httpContext,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            );

            return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }


        [Fact]
        public void Returns500_WhenJwtHelperIsNull()
        {
            var context = CreateContext("Bearer token", null!);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            Assert.IsType<StatusCodeResult>(context.Result);
            Assert.Equal(500, ((StatusCodeResult)context.Result).StatusCode);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Token token")]
        public void Returns401_WhenAuthHeaderIsMissingOrInvalid(string header)
        {
            var jwtMock = new Mock<IJwtHelper>();
            var context = CreateContext(header, jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<UnauthorizedObjectResult>(context.Result);
            Assert.Contains("missing or invalid", result.Value?.ToString());
        }

        [Fact]
        public void Returns401_WhenTokenIsInvalid()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(false);

            var context = CreateContext("Bearer invalidtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<UnauthorizedObjectResult>(context.Result);
            Assert.Contains("invalid or expired", result.Value?.ToString());
        }

        [Fact]
        public void Returns403_WhenRoleIsMissingOrNotAllowed()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
            jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>()))
                .Returns(new List<Claim> { new Claim("Roles", "User") });

            var context = CreateContext("Bearer validtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<ForbidResult>(context.Result);
            Assert.Equal("You are not authorized to perform this action.", ((ForbidResult)result).AuthenticationSchemes.First());
        }

        [Fact]
        public void Returns403_WhenClaimsAreNull()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
            jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>())).Returns((IEnumerable<Claim>?)null);

            var context = CreateContext("Bearer validtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<ForbidResult>(context.Result);
            Assert.Equal("You are not authorized to perform this action.", result.AuthenticationSchemes.First());
        }


        [Fact]
        public void Returns403_WhenRolesClaimIsMissing()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
            jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>()))
                .Returns(new List<Claim> { new Claim("Other", "Data") });

            var context = CreateContext("Bearer validtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<ForbidResult>(context.Result);
            Assert.Equal("You are not authorized to perform this action.", ((ForbidResult)result).AuthenticationSchemes.First());
        }

        [Fact]
        public void Returns403_WhenRolesClaimIsEmpty()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
            jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>()))
                .Returns(new List<Claim> { new Claim("Roles", "") });

            var context = CreateContext("Bearer validtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            var result = Assert.IsType<ForbidResult>(context.Result);
            Assert.Equal("You are not authorized to perform this action.", ((ForbidResult)result).AuthenticationSchemes.First());
        }

        [Fact]
        public void AllowsAccess_WhenRoleIsAllowed()
        {
            var jwtMock = new Mock<IJwtHelper>();
            jwtMock.Setup(j => j.ValidateJwtToken(It.IsAny<string>())).Returns(true);
            jwtMock.Setup(j => j.GetClaimsFromJwt(It.IsAny<string>()))
                .Returns(new List<Claim> { new Claim("Roles", "Admin,User") });

            var context = CreateContext("Bearer validtoken", jwtMock.Object);
            var attribute = new RoleAuthorizeAttribute("Admin");

            attribute.OnAuthorization(context);

            Assert.Null(context.Result);
        }
    }
}