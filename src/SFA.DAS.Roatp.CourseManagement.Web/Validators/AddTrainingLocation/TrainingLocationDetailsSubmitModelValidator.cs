using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation
{
    public class TrainingLocationDetailsSubmitModelValidator : AbstractValidator<ProviderLocationDetailsSubmitModel>
    {
        public const string VenueNameMissingMessage = "Enter a venue name";
        public const string VenueNameCharacterLimitMessage = "Venue name must be 50 characters or less";

        public TrainingLocationDetailsSubmitModelValidator()
        {
            RuleFor(m => m.LocationName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(VenueNameMissingMessage)
                .MaximumLength(50)
                .WithMessage(VenueNameCharacterLimitMessage);
        }
    }
}
