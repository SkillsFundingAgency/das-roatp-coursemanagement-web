using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class CourseContactDetailsSubmitModelValidator: AbstractValidator<CourseContactDetailsSubmitModel>
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
                .NotEmpty()
                .WithMessage(CommonValidationErrorMessage.TelephoneMissingMessage)
                .MinimumLength(10)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage)
                .MaximumLength(50)
                .WithMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
            RuleFor(p => p.ContactUsPageUrl)
                .NotEmpty()
                .WithMessage("Enter a contact page link")
                .MaximumLength(500)
                .WithMessage("Contact page address must be 500 characters or fewer")
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage("Enter an address in the correct format, like www.example.com");
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
