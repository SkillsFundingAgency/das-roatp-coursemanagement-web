using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthenticationProviderExtensions
    {
        private const string SignedOutCallbackPath = "/signout";
        public static void AddAndConfigureProviderAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddAndConfigureDfESignInAuthentication(configuration,
                "SFA.DAS.ProviderApprenticeshipService",
                typeof(CustomServiceRole),
                ClientName.ProviderRoatp,
                SignedOutCallbackPath,
                "");
        }
    }
}
