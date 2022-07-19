using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.AddressSubmitModelValidatorTests
{
    [TestFixture]
    public class SelectedAddressValidationTests
    {
        [Test]
        public void SelectedAddress_NullOrEmpty_IsInvalid()
        {
            var sut = new AddressSubmitModelValidator();
            var model = new AddressSubmitModel();

            var result = sut.TestValidate(model);

            result
                .ShouldHaveValidationErrorFor(a => a.SelectedAddressUprn)
                .WithErrorMessage(AddressSubmitModelValidator.AddressNotSelectedErrorMessage);
        }
    }
}
