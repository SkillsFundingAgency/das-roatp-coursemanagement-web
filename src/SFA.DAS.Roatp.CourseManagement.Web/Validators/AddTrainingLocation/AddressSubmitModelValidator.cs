using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation
{
    public class AddressSubmitModelValidator : AbstractValidator<AddressSubmitModel>
    {
        public const string AddressNotSelectedErrorMessage = "You must choose an address from the list";
        public AddressSubmitModelValidator()
        {
            RuleFor(m => m.SelectedAddressUprn)
                .NotEmpty()
                .WithMessage(AddressNotSelectedErrorMessage);
        }
    }
}
