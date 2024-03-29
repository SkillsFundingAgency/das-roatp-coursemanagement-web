﻿using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.CourseContactDetailsSubmitModelValidatorTests
{
    [TestFixture]
    public class ValidatePhoneNumberTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void WhenEmpty_ProducesValidatonError(string phoneNumber)
        {
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPhoneNumber = phoneNumber
            };

            var result = sut.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber)
                .WithErrorMessage("Enter a UK telephone number");
        }

        [Test]
        public void WhenTooLong_ProducesValidatonError()
        {
            string phoneNumber = new string('1', 51);
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPhoneNumber = phoneNumber
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber).WithErrorMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
        }

        [Test]
        public void WhenTooShort_ProducesValidatonError()
        {
            string phoneNumber = new string('1', 9);
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPhoneNumber = phoneNumber
            };

            var result = sut.TestValidate(command);

            result.ShouldHaveValidationErrorFor(c => c.ContactUsPhoneNumber).WithErrorMessage(CommonValidationErrorMessage.TelephoneLengthMessage);
        }

        [TestCase("01234 5678")]
        [TestCase("0123456789")]
        [TestCase("01234567890 ext 1234")]
        public void ValidValues_PassValidation(string phoneNumber)
        {
            var sut = new CourseContactDetailsSubmitModelValidator();

            var command = new EditCourseContactDetailsViewModel()
            {
                ContactUsPhoneNumber = phoneNumber
            };

            var result = sut.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(c => c.ContactUsPhoneNumber);
        }
    }
}
