using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AddDataProtectionExtension
    {
        public static void AddDataProtection(this IServiceCollection services, IConfiguration configuration)
        {

            var config = configuration.GetSection(nameof(RoatpCourseManagement))
                .Get<RoatpCourseManagement>();

            if (config != null
                && !string.IsNullOrEmpty(config.DataProtectionKeysDatabase)
                && !string.IsNullOrEmpty(config.RedisConnectionString))
            {
                var redisConnectionString = config.RedisConnectionString;
                var dataProtectionKeysDatabase = config.DataProtectionKeysDatabase;

                var redis = ConnectionMultiplexer
                    .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                services.AddDataProtection()
                    .SetApplicationName("das-roatp-coursemanagement-web")
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }
        }
    }
}
