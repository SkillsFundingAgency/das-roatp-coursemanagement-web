using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    public class ProviderAuthorizationHandler : AuthorizationHandler<ProviderUkPrnRequirement>
    {
        private const string ukprnRootValue = "ukprn";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkPrnRequirement requirement)
        {
            if (!IsProviderAuthorised(context))
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);

            return Task.CompletedTask;
        }

        private bool IsProviderAuthorised(AuthorizationHandlerContext context)
        {
            if (!context.User.HasClaim(c => c.Type.Equals(ProviderClaims.ProviderUkprn)))
            {
                return false;
            }

            if (_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey(ukprnRootValue))
            {
                var ukPrnFromUrl = _httpContextAccessor.HttpContext.Request.RouteValues[ukprnRootValue].ToString();
                var ukPrn = context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;

                return ukPrn.Equals(ukPrnFromUrl);
            }

            return true;
        }
    }
}
