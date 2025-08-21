using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddProviderContact;

public class
    AddProviderContactStandardsSubmitViewModelValidator : AbstractValidator<AddProviderContactStandardsSubmitViewModel>
{
    public const string SelectAStandardErrorMessage = "You must select at least one standard";

    public AddProviderContactStandardsSubmitViewModelValidator()
    {
        RuleFor(p => p.SelectedProviderCourseIds)
            .NotEmpty()
            .WithMessage(SelectAStandardErrorMessage);
    }
}
