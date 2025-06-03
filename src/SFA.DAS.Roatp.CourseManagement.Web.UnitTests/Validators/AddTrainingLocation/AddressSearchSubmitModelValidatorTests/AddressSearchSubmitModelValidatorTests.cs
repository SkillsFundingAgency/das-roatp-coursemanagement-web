using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.AddressSearchSubmitModelValidatorTests;
public class AddressSearchSubmitModelValidatorTests
{
    [TestFixture]
    public class AddressSearchSubmitValidationTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void PostcodeIsRequired(string postcode)
        {
            var model = new AddressSearchSubmitModel { Postcode = postcode };

            var sut = new AddressSearchSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.SearchTerm).WithErrorMessage(AddressSearchSubmitModelValidator.MissingAddressErrorMessage);
        }

        [Test]
        public void PostcodeIsPresent()
        {
            var model = new AddressSearchSubmitModel { Postcode = "CV1" };
            var sut = new AddressSearchSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
