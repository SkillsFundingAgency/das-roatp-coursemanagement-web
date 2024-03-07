using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthenticationProviderExtensions
    {
        private const string SignedOutCallbackPath = "/signout";
        public static void AddAndConfigureProviderAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var roatpCourseManagementConfiguration = configuration
                .GetSection(nameof(RoatpCourseManagement))
                .Get<RoatpCourseManagement>();

            services
            .AddAndConfigureDfESignInAuthentication(configuration,
                "SFA.DAS.ProviderApprenticeshipService",
                typeof(CustomServiceRole),
                ClientName.ProviderRoatp,
                SignedOutCallbackPath,
                "");
        }

        private static Task PopulateProviderClaims(HttpContext httpContext, ClaimsPrincipal principal)
        {
            var ukprn = principal.Claims.First(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value;
            var displayName = principal.Claims.First(c => c.Type.Equals(ProviderClaims.DisplayName)).Value;
            httpContext.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukprn);
            httpContext.Items.Add(ProviderClaims.DisplayName, displayName);

            principal.Identities.First().AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukprn));
            principal.Identities.First().AddClaim(new Claim(ProviderClaims.DisplayName, displayName));
            return Task.CompletedTask;
        }
    }
}
