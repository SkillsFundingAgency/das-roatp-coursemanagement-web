using System.ComponentModel;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

public enum ApprenticeshipType
{
    [Description("Standard apprenticeship")]
    Apprenticeship,
    [Description("Foundation apprenticeship")]
    FoundationApprenticeship
}