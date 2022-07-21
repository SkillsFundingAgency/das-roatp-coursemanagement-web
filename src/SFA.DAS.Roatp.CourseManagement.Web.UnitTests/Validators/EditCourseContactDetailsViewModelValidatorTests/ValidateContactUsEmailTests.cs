using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.EditCourseContactDetailsViewModelValidatorTests
{
    [TestFixture]
    public class ValidateContactUsEmailTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenEmpty_ProducesValidatonError(string email)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage("Enter an email address");
        }

        [Test]
        public void WhenTooLong_ProducesValidatonError()
        {
            string email = new string('*', 257);
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage("Email address must be 256 characters or fewer");
        }


        [TestCase("dfdfsd")]
        [TestCase("dfdfsd@")]
        [TestCase("dfdfsd@xv")]
        [TestCase("@rrfdsg")]
        [TestCase("w@w.")]
        public void WhenInvalid_ProducesValidatonError(string email)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage("Enter an email address in the correct format, like name@example.com");
        }

        [TestCase("q@q.q")]
        [TestCase("2@3.4")]
        [TestCase("helpdesk@20_apprenticeships.service.gov.uk")]
        public void WhenValid_ShouldNotHaveErrorForEmail(string email)
        {
            var sut = new EditCourseContactDetailsViewModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactUsEmail);
        }
    }
}
