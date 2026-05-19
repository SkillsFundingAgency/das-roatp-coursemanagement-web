using System.ComponentModel;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

public enum LearningType
{
    [Description("Standard")]
    Apprenticeship,
    [Description("Foundation apprenticeship")]
    FoundationApprenticeship,
    [Description("Apprenticeship unit")]
    ApprenticeshipUnit,
}