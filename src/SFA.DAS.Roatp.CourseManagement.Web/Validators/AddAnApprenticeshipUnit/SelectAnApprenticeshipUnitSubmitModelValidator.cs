using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAnApprenticeshipUnit;

public class SelectAnApprenticeshipUnitSubmitModelValidator : AbstractValidator<SelectAnApprenticeshipUnitSubmitModel>
{
    public const string ApprenticeshipUnitIsRequiredMesssage = "Select an apprenticeship unit";
    public SelectAnApprenticeshipUnitSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLarsCode)
            .NotEmpty()
            .WithMessage(ApprenticeshipUnitIsRequiredMesssage);
    }
}
