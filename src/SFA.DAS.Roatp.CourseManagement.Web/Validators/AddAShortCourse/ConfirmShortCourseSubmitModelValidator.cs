using FluentValidation;
using Humanizer;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAShortCourse;

public class ConfirmShortCourseSubmitModelValidator : AbstractValidator<ConfirmShortCourseSubmitModel>
{
    public ConfirmShortCourseSubmitModelValidator()
    {
        RuleFor(c => c.IsCorrectShortCourse)
            .NotEmpty()
            .WithMessage(c => $"You must select if this is the right {c.CourseType} or not".Humanize());
    }
}