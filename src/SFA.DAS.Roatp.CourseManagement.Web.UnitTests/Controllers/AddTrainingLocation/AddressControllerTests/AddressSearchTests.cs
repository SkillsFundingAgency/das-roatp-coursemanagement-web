using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
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
        Mock<ITempDataDictionary> tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        var addressSearch = sut.AddressSearch();
        addressSearch.Result.As<ViewResult>().Should().NotBeNull();
        addressSearch.Result.As<ViewResult>().ViewName.Should().Be(AddressController.ViewPath);
    }
}
