using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class CourseContactDetailsSubmitModelValidator : AbstractValidator<CourseContactDetailsSubmitModel>
    {
        public CourseContactDetailsSubmitModelValidator()
        {
            RuleFor(p => p.ContactUsEmail)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.EmailMissingMessage)
                .MaximumLength(256)
                .WithMessage(CommonValidationErrorMessage.EmailLengthMessage)
                .Matches(Constants.RegularExpressions.EmailRegex)
                .WithMessage(CommonValidationErrorMessage.EmailInvalidMessage);

            RuleFor(p => p.ContactUsPhoneNumber)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.TelephoneMissingMessage)
                .Matches(Constants.RegularExpressions.ExcludedCharactersRegex)
                .WithMessage(CommonValidationErrorMessage.TelephoneHasExcludedCharacter)
                .MinimumLength(10)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage)
                .MaximumLength(50)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage);

            RuleFor(p => p.StandardInfoUrl)
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.WebsiteMissingMessage)
                .MaximumLength(500)
                .WithMessage(CommonValidationErrorMessage.WebsiteLengthMessage)
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage(CommonValidationErrorMessage.WebsiteInvalidMessage);
        }
    }
}
