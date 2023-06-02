using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization
{
    public class CustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => ProviderClaims.Service;
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Code;
    }
}
