using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class LocationOptionSubmitModelValidator : AbstractValidator<LocationOptionSubmitModel>
    {
        public const string NoneSelectedErrorMessage = "Select where you will deliver this standard";

        public LocationOptionSubmitModelValidator()
        {
            RuleFor(m => m.LocationOption)
                .NotEqual(LocationOption.None)
                .WithMessage(NoneSelectedErrorMessage);
        }
    }
}
