using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses.AddAShortCourse;

public class SelectShortCourseSubmitModelValidator : AbstractValidator<SelectShortCourseSubmitModel>
{
    public SelectShortCourseSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLarsCode)
            .NotEmpty()
            .WithMessage(m => $"Select an {m.ApprenticeshipTypeLower}");
    }
}
