using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

public class SelectShortCourseLocationOptionsSubmitModelValidator : AbstractValidator<SelectShortCourseLocationOptionsSubmitModel>
{
    public const string NoneSelectedErrorMessage = "Select at least one delivery method";

    public SelectShortCourseLocationOptionsSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLocationOptions)
            .NotEmpty()
            .WithMessage(NoneSelectedErrorMessage);
    }
}