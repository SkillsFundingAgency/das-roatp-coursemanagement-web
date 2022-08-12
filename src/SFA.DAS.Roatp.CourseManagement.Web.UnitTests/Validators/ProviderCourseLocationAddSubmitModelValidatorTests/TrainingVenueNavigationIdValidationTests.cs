using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ProviderCourseLocationAddSubmitModelValidatorTests
{
    [TestFixture]
    public class TrainingVenueNavigationIdValidationTests
    {
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void TrainingVenueNavigationId_NullOrEmpty_IsInValid(string trainingVenueNavigation)
        {
            var sut = new ProviderCourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(new ProviderCourseLocationAddSubmitModel() { TrainingVenueNavigationId = trainingVenueNavigation });

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(m => m.TrainingVenueNavigationId).WithErrorMessage(ProviderCourseLocationAddSubmitModelValidator.TrainingVenueErrorMessage);
        }
    }
}
