﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationServicePolicyExtension
    {
        private const string ProviderDaa = "DAA";
        private const string ProviderDab = "DAB";
        private const string ProviderDac = "DAC";
        private const string ProviderDav = "DAV";

        public static void AddAuthorizationServicePolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    PolicyNames.HasProviderAccount
                    , policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim(ProviderClaims.ProviderUkprn);
                        policy.RequireClaim(ProviderClaims.Service, ProviderDaa, ProviderDab, ProviderDac, ProviderDav);
                        policy.Requirements.Add(new ProviderUkPrnRequirement());
                        policy.Requirements.Add(new TrainingProviderAllRolesRequirement());
                    });
            });
        }
    }
}
