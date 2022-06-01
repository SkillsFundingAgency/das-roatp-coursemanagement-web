using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.EditCourseContactDetailsViewModelValidatorTests
{
    [TestFixture]
    public class ValidateContactUsPageUrlTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenEmpty_ProducesValidatonError(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPageUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl).WithErrorMessage("Enter a contact page link");
        }

        [Test]
        public void WhenTooLong_ProducesValidatonError()
        {
            string url = new string('q', 501);
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPageUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl).WithErrorMessage("Contact page address must be 500 characters or fewer");
        }

        [TestCase("dfdfsd")]
        public void WhenInvalid_ProducesValidatonError(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPageUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsPageUrl).WithErrorMessage("Enter an address in the correct format, like www.example.com");
        }

        [TestCase("dfdfsd.sd")]
        [TestCase("dfdfsd.sd.sfdg")]
        public void WhenValid_ShouldNotHaveErrorForEmail(string url)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPageUrl = url
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPageUrl);
        }
    }
}
