using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddShortCourses;

public class SelectShortCourseSubmitModelValidator : AbstractValidator<SelectShortCourseSubmitModel>
{
    public SelectShortCourseSubmitModelValidator()
    {
        RuleFor(m => m.SelectedLarsCode)
            .NotEmpty()
            .WithMessage(m => $"Select a {m.CourseTypeDescription}");
    }
}
