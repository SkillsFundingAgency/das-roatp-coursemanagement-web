using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
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

            RuleFor(c => c.EmailAddress)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.EmailMissingMessage)
                .MaximumLength(256)
                .WithMessage(CommonValidationErrorMessage.EmailLengthMessage)
                .Matches(Constants.RegularExpressions.EmailRegex)
                .WithMessage(CommonValidationErrorMessage.EmailInvalidMessage);

            RuleFor(c => c.PhoneNumber)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.TelephoneMissingMessage)
                .MinimumLength(10)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage)
                .MaximumLength(50)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage);

            RuleFor(c => c.Website)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.WebsiteMissingMessage)
                .MaximumLength(500)
                .WithMessage(CommonValidationErrorMessage.WebsiteLengthMessage)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage(CommonValidationErrorMessage.WebsiteInvalidMessage);
        }
    }
}
