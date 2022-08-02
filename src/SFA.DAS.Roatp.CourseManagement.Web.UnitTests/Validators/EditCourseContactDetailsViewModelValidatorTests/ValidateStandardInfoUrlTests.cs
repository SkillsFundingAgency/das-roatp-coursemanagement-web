using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.EditCourseContactDetailsViewModelValidatorTests
{
    [TestFixture]
    public class ValidateStandardInfoUrlTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenEmpty_ProducesValidatonError(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                StandardInfoUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(CommonValidationErrorMessage.WebsiteMissingMessage);
        }

        [Test]
        public void WhenTooLong_ProducesValidatonError()
        {
            string url = new string('q', 501);
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                StandardInfoUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(CommonValidationErrorMessage.WebsiteLengthMessage);
        }

        [TestCase("dfdfsd")]
        public void WhenInvalid_ProducesValidatonError(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                StandardInfoUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.StandardInfoUrl).WithErrorMessage(CommonValidationErrorMessage.WebsiteInvalidMessage);
        }

        [TestCase("dfdfsd.sd")]
        [TestCase("dfdfsd.sd.sfdg")]
        public void WhenValid_ShouldNotHaveErrorForEmail(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                StandardInfoUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.StandardInfoUrl);
        }
    }
}
