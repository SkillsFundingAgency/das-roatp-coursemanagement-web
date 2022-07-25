using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class WebsiteValidationTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void IsRequired(string website)
        {
            var model = new ProviderLocationDetailsSubmitModel { Website = website };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.Website).WithErrorMessage(CommonValidationErrorMessage.WebsiteMissingMessage);
        }

        [Test]
        public void ShouldNotExceedCharacterLimitOf500()
        {
            var model = new ProviderLocationDetailsSubmitModel { Website = $"{new string('a', 497)}.com" };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.Website).WithErrorMessage(CommonValidationErrorMessage.WebsiteLengthMessage);
        }

        [Test]
        public void ShouldBeAValidUrlFormat()
        {
            var model = new ProviderLocationDetailsSubmitModel { Website = "aacom" };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.Website).WithErrorMessage(CommonValidationErrorMessage.WebsiteInvalidMessage);
        }
    }
}
