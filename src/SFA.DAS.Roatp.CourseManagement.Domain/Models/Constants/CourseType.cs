using System.ComponentModel;

namespace SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;

public enum CourseType
{
    [Description("Standard apprenticeship")]
    Apprenticeship,
    [Description("Apprenticeship unit")]
    ApprenticeshipUnit
}