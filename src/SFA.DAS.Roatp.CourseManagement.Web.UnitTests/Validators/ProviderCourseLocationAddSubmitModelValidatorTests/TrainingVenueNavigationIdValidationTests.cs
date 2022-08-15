using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;
using System;

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

      
        [Test]
        public void TrainingVenueNavigationId_ValidId_ReturnsNoErrors()
        {
            var sut = new ProviderCourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(new ProviderCourseLocationAddSubmitModel() { TrainingVenueNavigationId = Guid.NewGuid().ToString(), HasDayReleaseDeliveryOption= true });

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(m => m.TrainingVenueNavigationId);
        }
    }
}
