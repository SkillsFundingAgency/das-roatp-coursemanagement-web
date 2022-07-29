using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.TrainingLocationDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class LocationNameValidationTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void IsRequired(string locationName)
        {
            var model = new ProviderLocationDetailsSubmitModel { LocationName = locationName };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(TrainingLocationDetailsSubmitModelValidator.VenueNameMissingMessage);
        }

        [Test]
        public void ShouldNotExceedCharacterLimitOf50()
        {
            var model = new ProviderLocationDetailsSubmitModel { LocationName = new string('a', 51) };

            var sut = new TrainingLocationDetailsSubmitModelValidator();
            var result = sut.TestValidateAsync(model).Result;
            result.ShouldHaveValidationErrorFor(m => m.LocationName).WithErrorMessage(TrainingLocationDetailsSubmitModelValidator.VenueNameCharacterLimitMessage);
        }
    }
}
