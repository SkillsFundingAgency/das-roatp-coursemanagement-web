using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class TrainingLocationListViewModelValidator : AbstractValidator<TrainingLocationListViewModel>
    {
        public const string TrainingLocationErrorMessage = "You must add a training location";
        public TrainingLocationListViewModelValidator()
        {
            RuleFor(p => p.ProviderCourseLocations).NotEmpty()
                .WithMessage(TrainingLocationErrorMessage);
        }
    }
}
