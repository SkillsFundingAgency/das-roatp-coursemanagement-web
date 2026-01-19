using FluentValidation;
using Humanizer;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAShortCourse;

public class SelectShortCourseSubmitModelValidator : AbstractValidator<SelectShortCourseSubmitModel>
{
    public SelectShortCourseSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLarsCode)
            .NotEmpty()
            .WithMessage(m => $"Select an {m.CourseType}".Humanize());
    }
}
