using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ProviderCourseLocationAddSubmitModelValidatorTests
{
    [TestFixture]
    public class DeliveryMethodValidationTests
    {
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void Validate_DeliveryMethod_IsInValid(bool hasDayReleaseDeliveryOption, bool hasBlockReleaseDeliveryOption, bool isvalid)
        {
            var sut = new ProviderCourseLocationAddSubmitModelValidator();

            var result = sut.TestValidate(new ProviderCourseLocationAddSubmitModel() { HasDayReleaseDeliveryOption = hasDayReleaseDeliveryOption, HasBlockReleaseDeliveryOption  = hasBlockReleaseDeliveryOption, TrainingVenueNavigationId ="someId" });

            if(isvalid)
            {
                result.IsValid.Should().BeTrue();
                result.ShouldNotHaveValidationErrorFor(m => m.HasDayReleaseDeliveryOption);
            }
            else
            {
                result.IsValid.Should().BeFalse();
                result.ShouldHaveValidationErrorFor(m => m.HasDayReleaseDeliveryOption).WithErrorMessage(ProviderCourseLocationAddSubmitModelValidator.DeliveryMethodErrorMessage);
            }
        }
    }
}
