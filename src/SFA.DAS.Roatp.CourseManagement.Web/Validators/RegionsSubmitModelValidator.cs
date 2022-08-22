using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class RegionsSubmitModelValidator : AbstractValidator<RegionsSubmitModel>
    {
        public const string SelectSubRegionsErrorMessage = "Select the regions where you can deliver this training";
        public RegionsSubmitModelValidator()
        {
            RuleFor(p => p.SelectedSubRegions)
                .NotNull()
                .WithMessage(SelectSubRegionsErrorMessage)
                .NotEmpty()
                .WithMessage(SelectSubRegionsErrorMessage);
        }
    }
}
