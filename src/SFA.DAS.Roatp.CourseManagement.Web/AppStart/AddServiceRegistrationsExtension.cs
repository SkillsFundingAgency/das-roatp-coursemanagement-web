using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Roatp.CourseManagement.Infrastructure.ApiClients;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    public static class AddServiceRegistrationsExtension
    {
        public static void AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureHttpClient(services, configuration);
            services.AddHttpContextAccessor();
            services.AddTransient<ISessionService, SessionService>();
        }
        private static void ConfigureHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            var handlerLifeTime = TimeSpan.FromMinutes(5);
            services.AddHttpClient<IApiClient, ApiClient>(config =>
            {
                var outerApiConfiguration = configuration
                    .GetSection(nameof(RoatpCourseManagementOuterApi))
                    .Get<RoatpCourseManagementOuterApi>();

                config.BaseAddress = new Uri(outerApiConfiguration.BaseUrl);
                config.DefaultRequestHeaders.Add("Accept", "application/json");
                config.DefaultRequestHeaders.Add("X-Version", "1");
                config.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", outerApiConfiguration.SubscriptionKey);
            })
           .SetHandlerLifetime(handlerLifeTime);
        }
    }
}
