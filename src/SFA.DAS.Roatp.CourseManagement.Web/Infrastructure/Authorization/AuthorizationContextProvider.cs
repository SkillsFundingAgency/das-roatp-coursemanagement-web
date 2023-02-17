using Microsoft.AspNetCore.Http;
using SFA.DAS.Authorization.Context;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    [ExcludeFromCodeCoverage]
    public class AuthorizationContextProvider : IAuthorizationContextProvider
    {
        private readonly IHttpContextAccessor _httpContextAssessor;

        public AuthorizationContextProvider(IHttpContextAccessor httpContextAssessor)
        {
            _httpContextAssessor = httpContextAssessor;
        }

        public IAuthorizationContext GetAuthorizationContext()
        {
            return new AuthorizationContext();
        }
    }
}
