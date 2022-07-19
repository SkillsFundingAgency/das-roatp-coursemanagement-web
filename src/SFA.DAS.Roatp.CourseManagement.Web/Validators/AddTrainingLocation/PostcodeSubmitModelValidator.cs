using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System.Text.RegularExpressions;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation
{
    public class PostcodeSubmitModelValidator : AbstractValidator<PostcodeSubmitModel>
    {
        public const string PostcodeEmptyMessage = "You must enter a postcode";
        public const string PostcodeInvalidMessage = "Enter a real postcode";
        public const string PostcodeRegex = @"^[a-z]{1,2}\d[a-z\d]?\s*\d[a-z]{2}$";
        public PostcodeSubmitModelValidator()
        {
            RuleFor(m => m.Postcode)
                .NotEmpty()
                .WithMessage(PostcodeEmptyMessage)
                .Matches(PostcodeRegex, RegexOptions.IgnoreCase)
                .WithMessage(PostcodeInvalidMessage);
        }
    }
}
