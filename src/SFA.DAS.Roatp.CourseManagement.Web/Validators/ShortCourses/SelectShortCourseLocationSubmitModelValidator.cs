using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

public class SelectShortCourseLocationSubmitModelValidator : AbstractValidator<SelectShortCourseLocationSubmitModel>
{
    public const string NoneSelectedErrorMessage = "Select at least one delivery method";

    public SelectShortCourseLocationSubmitModelValidator()
    {
        RuleFor(m => m.ShortCourseLocations)
            .NotEmpty()
            .WithMessage(NoneSelectedErrorMessage);
    }
}