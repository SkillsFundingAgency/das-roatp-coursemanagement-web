using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddSingleton(cfg => cfg.GetService<IOptions<RoatpCourseManagement>>().Value);
            services.Configure<RoatpCourseManagementOuterApi>(configuration.GetSection(nameof(RoatpCourseManagementOuterApi)));
            services.AddSingleton(cfg => cfg.GetService<IOptions<RoatpCourseManagementOuterApi>>().Value);
        }
    }
}
