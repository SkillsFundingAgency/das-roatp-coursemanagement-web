using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public class RegionsViewModelValidator : AbstractValidator<RegionsViewModel>
    {
        public const string SelectSubRegionsErrorMessage = "Select the regions where you can deliver this training";
        public RegionsViewModelValidator()
        {
            RuleFor(p => p.SelectedSubRegions)
                .NotNull()
                .WithMessage(SelectSubRegionsErrorMessage)
                .NotEmpty()
                .WithMessage(SelectSubRegionsErrorMessage);
        }
    }
}
