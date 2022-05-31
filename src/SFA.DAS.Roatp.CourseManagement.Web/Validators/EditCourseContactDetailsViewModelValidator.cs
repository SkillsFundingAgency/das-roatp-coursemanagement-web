using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class EditCourseContactDetailsViewModelValidator: AbstractValidator<EditCourseContactDetailsViewModel>
    {
        public const string TelephoneLengthErrorMessage = "Telephone number must be between 10 and 50 characters";
        public EditCourseContactDetailsViewModelValidator()
        {
            RuleFor(p => p.ContactUsEmail)
                .NotEmpty()
                .WithMessage("Enter an email address")
                .MaximumLength(256)
                .WithMessage("Email address must be 256 characters or fewer")
                .Matches(Constants.RegularExpressions.EmailRegex)
                .WithMessage("Enter an email address in the correct format, like name@example.com");
            RuleFor(p => p.ContactUsPhoneNumber)
                .NotEmpty()
                .WithMessage("Enter a UK telephone number")
                .MinimumLength(10)
                .WithMessage(TelephoneLengthErrorMessage)
                .MaximumLength(50)
                .WithMessage(TelephoneLengthErrorMessage);
            RuleFor(p => p.ContactUsPageUrl)
                .NotEmpty()
                .WithMessage("Enter a contact page link")
                .MaximumLength(500)
                .WithMessage("Contact page address must be 500 characters or fewer")
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage("Enter an address in the correct format, like www.example.com");
            RuleFor(p => p.StandardInfoUrl)
                .NotEmpty()
                .WithMessage("Enter a website page link")
                .MaximumLength(500)
                .WithMessage("Website address must be 500 characters or fewer")
                .Matches(Constants.RegularExpressions.UrlRegex)
                .WithMessage("Enter an address in the correct format, like www.example.com");
        }
    }
}
