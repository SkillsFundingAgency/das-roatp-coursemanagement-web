using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;


namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class ProviderCourseLocationListViewModelValidator : AbstractValidator<ProviderCourseLocationListViewModel>
    {
        public const string TrainingLocationErrorMessage = "You must add a training location";
        public ProviderCourseLocationListViewModelValidator()
        {
            RuleFor(p => p.ProviderCourseLocations).NotEmpty()
               .WithMessage(TrainingLocationErrorMessage);
        }
    }
}
