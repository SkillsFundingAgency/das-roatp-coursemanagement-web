using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Validators.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Validators.ShortCourses;
public class ConfirmNationalProviderDeliverySubmitModelValidatorTests
{
    [Test]
    public void HasNationalDeliveryOption_ReturnsInvalid()
    {
        var sut = new ConfirmNationalDeliverySubmitModelValidator();
        var model = new ConfirmNationalDeliverySubmitModel();

        var result = sut.Validate(model);

        result.IsValid.Should().BeFalse();
    }

    [TestCase(true)]
    [TestCase(false)]
    public void HasNationalDeliveryOption_ReturnsValid(bool value)
    {
        var sut = new ConfirmNationalDeliverySubmitModelValidator();
        var model = new ConfirmNationalDeliverySubmitModel() { HasNationalDeliveryOption = value };

        var result = sut.Validate(model);

        result.IsValid.Should().BeTrue();
    }

}
