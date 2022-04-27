using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<RoatpCourseManagement>(configuration.GetSection(nameof(RoatpCourseManagement)));
            services.Configure<RoatpCourseManagementOuterApi>(configuration.GetSection(nameof(RoatpCourseManagementOuterApi)));
            services.Configure<ProviderSharedUIConfiguration>(configuration.GetSection(nameof(ProviderSharedUIConfiguration)));
        }
    }
}
