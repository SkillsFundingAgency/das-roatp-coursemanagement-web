using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    public static class ProviderClaims
    {
        public static readonly string ProviderUkprn = "http://schemas.portal.com/ukprn";
        public static readonly string DisplayName = "http://schemas.portal.com/displayname";
        public static readonly string Service = "http://schemas.portal.com/service";
        public static readonly string UserId = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn";
        public static readonly string Email = "http://schemas.portal.com/mail";
        public static readonly string DfEUserId = "sub";
    }
}
