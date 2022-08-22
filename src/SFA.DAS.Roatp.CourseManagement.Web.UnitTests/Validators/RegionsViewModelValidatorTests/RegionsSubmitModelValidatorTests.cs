using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ConfirmRegulatedStandardViewModelValidatorTests
{
    [TestFixture]
    public class RegionsSubmitModelValidatorTests
    {
        private const string SelectSubRegionsErrorMessage = "Select the regions where you can deliver this training";
        [Test]
        public void WhenNull_ProducesValidatonError()
        {
            var sut = new RegionsSubmitModelValidator();

            var command = new RegionsViewModel();

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.SelectedSubRegions).WithErrorMessage(SelectSubRegionsErrorMessage);
        }

        [Test]
        public void WhenValidModel_ProducesNoError()
        {
            var sut = new RegionsSubmitModelValidator();

            var command = new RegionsViewModel()
            {
                SelectedSubRegions = new string[] { "1", "2" }
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
