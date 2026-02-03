using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

public class SelectShortCourseTrainingVenuesSubmitModelValidator : AbstractValidator<SelectShortCourseTrainingVenuesSubmitModel>
{
    public const string NoneSelectedErrorMessage = "Select at least one training venue";

    public SelectShortCourseTrainingVenuesSubmitModelValidator()
    {
        RuleFor(m => m.SelectedProviderLocationIds)
            .NotEmpty()
            .WithMessage(NoneSelectedErrorMessage);
    }
}