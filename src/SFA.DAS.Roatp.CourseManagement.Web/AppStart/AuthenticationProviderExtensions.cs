using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;

namespace SFA.DAS.Roatp.CourseManagement.Web.AppStart
{
    [ExcludeFromCodeCoverage]
    public static class AuthenticationProviderExtensions
    {
        public static void AddAndConfigureProviderAuthentication(this IServiceCollection services, ProviderIdams idams)
        {
            var cookieOptions = new Action<CookieAuthenticationOptions>(options =>
            {
                options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.AccessDeniedPath = "/error/403";
            });

            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme =
                        CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme =
                        CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme =
                        WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = idams.MetadataAddress;
                    options.Wtrealm = idams.Wtrealm;
                    options.CallbackPath = "/{ukprn}/home"; 
                    options.Events.OnSecurityTokenValidated = async (ctx) =>
                    {
                        await PopulateProviderClaims(ctx.HttpContext, ctx.Principal);
                    };
                })
                .AddCookie(cookieOptions);

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
