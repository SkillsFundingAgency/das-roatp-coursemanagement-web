using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Validators;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ConfirmNationalProviderSubmitModelValidatorTests
{
    [TestFixture]
    public class HasNationalDeliveryOptionTests
    {
        [Test]
        public void HasNationalDeliveryOption_Empty_ReturnsInvalid()
        {
            var sut = new ConfirmNationalProviderSubmitModelValidator();
            var model = new ConfirmNationalProviderSubmitModel();

            var result = sut.Validate(model);

            result.IsValid.Should().BeFalse();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void HasNationalDeliveryOption_Empty_ReturnsInvalid(bool value)
        {
            var sut = new ConfirmNationalProviderSubmitModelValidator();
            var model = new ConfirmNationalProviderSubmitModel() { HasNationalDeliveryOption = value };

            var result = sut.Validate(model);

            result.IsValid.Should().BeTrue();
        }

    }
}
