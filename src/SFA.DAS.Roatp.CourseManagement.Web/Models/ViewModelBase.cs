using Microsoft.AspNetCore.Http;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public abstract class ViewModelBase
    {
        public Dictionary<string, string> RouteDictionary { get; } = new Dictionary<string, string>();
        protected ViewModelBase(HttpContext context)
        {
            RouteDictionary.Add("ukprn", context.User.FindFirst(c => c.Type.Equals(ProviderClaims.ProviderUkprn)).Value);
        }
    }
}
