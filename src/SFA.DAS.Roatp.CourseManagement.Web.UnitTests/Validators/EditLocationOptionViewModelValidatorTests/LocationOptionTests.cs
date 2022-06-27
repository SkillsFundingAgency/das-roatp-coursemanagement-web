using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.EditLocationOptionViewModelValidatorTests
{
    [TestFixture]
    public class LocationOptionTests
    {
        [Test]
        public void WhenNone_ReturnsValidationError()
        {
            var sut = new EditLocationOptionViewModelValidator();

            var model = new EditLocationOptionViewModel();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(c => c.LocationOption).WithErrorMessage(EditLocationOptionViewModelValidator.NoneSelectedErrorMessage);
        }
    }
}
