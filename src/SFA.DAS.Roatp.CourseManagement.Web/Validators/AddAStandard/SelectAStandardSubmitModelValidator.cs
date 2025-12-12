using FluentValidation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard
{
    public class SelectAStandardSubmitModelValidator : AbstractValidator<SelectAStandardSubmitModel>
    {
        public const string StandardIsRequiredMesssage = "Start typing the name of the standard in the box";
        public SelectAStandardSubmitModelValidator()
        {
            RuleFor(m => m.SelectedLarsCode)
                .NotEmpty()
                .WithMessage(StandardIsRequiredMesssage)
                .Must(larsCode => int.TryParse(larsCode, out var parsedLarsCode) && parsedLarsCode > 0)
                .WithMessage(StandardIsRequiredMesssage);
        }
    }
}
