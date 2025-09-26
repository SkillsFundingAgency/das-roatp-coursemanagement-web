using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddressControllerTests;

[TestFixture]
public class AddressSearchTests
{
    [Test]
    public void AddressSearch_ReturnsExpectedView()
    {
        var sut = new AddressController();
        sut.AddDefaultContextWithUser();
        var addressSearch = sut.AddressSearch();
        addressSearch.Result.As<ViewResult>().Should().NotBeNull();
        addressSearch.Result.As<ViewResult>().ViewName.Should().Be(AddressController.ViewPath);
    }
}
