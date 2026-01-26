using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.CourseContactDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class ValidateContactUsEmailTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenEmpty_ProducesValidatonError(string email)
        {
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(CommonValidationErrorMessage.EmailMissingMessage);
        }

        [Test]
        public void WhenTooLong_ProducesValidationError()
        {
            string email = new string('*', 257);
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(CommonValidationErrorMessage.EmailLengthMessage);
        }

        [TestCase("dfdfsd")]
        [TestCase("dfdfsd@")]
        [TestCase("dfdfsd@xv")]
        [TestCase("@rrfdsg")]
        [TestCase("w@w.")]
        public void WhenInvalid_ProducesValidationError(string email)
        {
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(CommonValidationErrorMessage.EmailInvalidMessage);
        }

        [Test]
        public void WhenDomainInvalid_ProducesValidationError()
        {
            string email = "aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk";

            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsEmail).WithErrorMessage(CommonValidationErrorMessage.EmailInvalidDomainMessage);
        }

        [TestCase("q@q.com")]
        [TestCase("2@3.4")]
        [TestCase("test@account.com")]
        [TestCase("test//@account.com")]
        public void WhenValid_ShouldNotHaveErrorForEmail(string email)
        {
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsEmail = email
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactUsEmail);
        }
    }
}
