using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Roatp.CourseManagement.Domain.Configuration;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.Controllers
{
    [ExcludeFromCodeCoverage]
    public class ProviderAccountController : Controller
    {
        private readonly IOptions<RoatpCourseManagement> _configOptions;

        public ProviderAccountController(IOptions<RoatpCourseManagement> configOptions)
        {
            _configOptions = configOptions;
        }

        [Route("signout", Name = RouteNames.ProviderSignOut)]
        public IActionResult SignOut()
        {
            var authScheme = _configOptions.Value.UseDfESignIn
                ? WsFederationDefaults.AuthenticationScheme
                : OpenIdConnectDefaults.AuthenticationScheme;

            return SignOut(
                new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    RedirectUri = "",
                    AllowRefresh = true
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                authScheme);
        }
    }
}
