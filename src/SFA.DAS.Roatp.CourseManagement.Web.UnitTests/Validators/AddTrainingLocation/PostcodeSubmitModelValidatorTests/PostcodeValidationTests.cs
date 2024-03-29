﻿using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.AddTrainingLocation.PostcodeSubmitModelValidatorTests
{
    [TestFixture]
    public class PostcodeValidationTests
    {
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Postcode_NullOrEmpty_IsInValid(string postcode)
        {
            var sut = new PostcodeSubmitModelValidator();

            var result = sut.TestValidate(new PostcodeSubmitModel() { Postcode = postcode });

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(m => m.Postcode).WithErrorMessage(PostcodeSubmitModelValidator.PostcodeEmptyMessage);
        }

        [TestCase("q")]
        [TestCase("as1")]
        [TestCase("124")]
        public void Postcode_IncorrectFormat_IsInValid(string postcode)
        {
            var sut = new PostcodeSubmitModelValidator();

            var result = sut.TestValidate(new PostcodeSubmitModel() { Postcode = postcode });

            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor(m => m.Postcode);
        }

        [TestCase("M1 1AA")]
        [TestCase("M60 1NW")]
        [TestCase("CR2 6HP")]
        [TestCase("DN55 1PT")]
        [TestCase("W1P 1HQ")]
        [TestCase("EC1A 1BB")]
        public void Postcode_CorrectFormat_IsValid(string postcode)
        {
            var sut = new PostcodeSubmitModelValidator();

            var result = sut.TestValidate(new PostcodeSubmitModel() { Postcode = postcode });

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(m => m.Postcode);
        }

        [TestCase("Ec1A 1bB")]
        [TestCase("ec1a 1bb")]
        public void Postcode_CorrectFormatInMixCase_IsValid(string postcode)
        {
            var sut = new PostcodeSubmitModelValidator();

            var result = sut.TestValidate(new PostcodeSubmitModel() { Postcode = postcode });

            result.IsValid.Should().BeTrue();
            result.ShouldNotHaveValidationErrorFor(m => m.Postcode);
        }
    }
}
