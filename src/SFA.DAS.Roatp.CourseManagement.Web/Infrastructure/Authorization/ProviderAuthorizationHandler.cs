using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    public class ProviderAuthorizationHandler : AuthorizationHandler<ProviderUkPrnRequirement>
    {
        private const string ukprnRootValue = "ukprn";
        private static readonly string[] _controllersThatDoNotRequireAuthorize = new[]
        {
            "PingController",
            "ProviderAccountController",
            "ErrorController",
            "ProviderNotRegisteredController",
        };
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProviderAuthorizationHandler(IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProviderUkPrnRequirement requirement)
        {
            if (!IsProviderAuthorised(context))
            {
                context.Fail();
                return;
            }

            await CheckIfProviderIsRegistered(_httpContextAccessor.HttpContext);

            context.Succeed(requirement);
        }

        private async Task CheckIfProviderIsRegistered(HttpContext context)
        {
            var controllerName = context.Request.RouteValues["controller"].ToString();
            if (_controllersThatDoNotRequireAuthorize.Any(n => n == controllerName)) return;
            var ukprn = int.Parse(context.User.FindFirstValue(ProviderClaims.ProviderUkprn));
            var provider = await _mediator.Send(new GetProviderQuery(ukprn));
            if (provider.Provider == null) context.Response.Redirect(RouteNames.ProviderNotRegistered);
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
