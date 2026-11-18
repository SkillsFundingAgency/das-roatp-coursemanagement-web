using System.ComponentModel;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Models;

public enum FundingStatus
{
    [Description("Funding closing for new starts")]
    FundingClosing,

    [Description("Funding closed for new starts")]
    FundingClosed
}
