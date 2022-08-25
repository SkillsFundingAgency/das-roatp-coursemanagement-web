 using System;
 using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddAStandard
{
    [TestFixture]
    public class CourseLocationAddSubmitModelValidatorTests
    {
        [Test]
        public void ValidModel_NoErrors()
        {
            var model = new CourseLocationAddSubmitModel {HasBlockReleaseDeliveryOption = true, TrainingVenueNavigationId = Guid.NewGuid().ToString()};
            var sut = new CourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldNotHaveValidationErrorFor(x=>x.TrainingVenueNavigationId);
            result.ShouldNotHaveValidationErrorFor(x => x.HasBlockReleaseDeliveryOption);
            result.ShouldNotHaveValidationErrorFor(x => x.HasDayReleaseDeliveryOption);
        }

        [Test]
        public void InvalidModel_NavigationError()
        {
            var model = new CourseLocationAddSubmitModel{ HasBlockReleaseDeliveryOption = true};
            var sut = new CourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.TrainingVenueNavigationId)
                .WithErrorMessage(CourseLocationAddSubmitModelValidator.TrainingVenueErrorMessage);
        }

        [Test]
        public void InvalidModel_DeliveryOptionError()
        {
            var model = new CourseLocationAddSubmitModel { TrainingVenueNavigationId = Guid.NewGuid().ToString() };
            var sut = new CourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.HasDayReleaseDeliveryOption)
                .WithErrorMessage(CourseLocationAddSubmitModelValidator.DeliveryMethodErrorMessage);
        }

        [Test]
        public void InvalidModel_NavigationAndDeliveryOptionError()
        {
            var model = new CourseLocationAddSubmitModel();
            var sut = new CourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.TrainingVenueNavigationId)
                .WithErrorMessage(CourseLocationAddSubmitModelValidator.TrainingVenueErrorMessage);
            result.ShouldHaveValidationErrorFor(x => x.HasDayReleaseDeliveryOption)
                .WithErrorMessage(CourseLocationAddSubmitModelValidator.DeliveryMethodErrorMessage);
        }
    }
}
