using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class EmailAddressValidationTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void IsRequired(string email)
        {
            var model = new ProviderLocationDetailsSubmitModel { EmailAddress = email };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.EmailAddress).WithErrorMessage(CommonValidationErrorMessage.EmailMissingMessage);
        }

        [Test]
        public void ShouldNotExceedCharacterLimitOf256()
        {
            var model = new ProviderLocationDetailsSubmitModel { EmailAddress = $"{new string('a', 250)}@aa.com"};

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.EmailAddress).WithErrorMessage(CommonValidationErrorMessage.EmailLengthMessage);
        }

        [Test]
        public void ShouldBeAValidEmailFormat()
        {
            var model = new ProviderLocationDetailsSubmitModel { EmailAddress = "aacom" };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.EmailAddress).WithErrorMessage(CommonValidationErrorMessage.EmailInvalidMessage);
        }
    }
}
