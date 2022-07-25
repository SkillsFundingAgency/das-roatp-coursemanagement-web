using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class PhoneNumberValidationTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void IsRequired(string phone)
        {
            var model = new ProviderLocationDetailsSubmitModel { PhoneNumber = phone };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber).WithErrorMessage(CommonValidationErrorMessage.TelephoneMissingMessage);
        }

        [Test]
        public void ShouldNotExceedCharacterLimitOf50()
        {
            var model = new ProviderLocationDetailsSubmitModel { PhoneNumber = new string('1', 51) };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber).WithErrorMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
        }

        [Test]
        public void ShouldNotBeLessThan10Characters()
        {
            var model = new ProviderLocationDetailsSubmitModel { PhoneNumber = new string('1', 9) };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.PhoneNumber).WithErrorMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
        }
    }
}
