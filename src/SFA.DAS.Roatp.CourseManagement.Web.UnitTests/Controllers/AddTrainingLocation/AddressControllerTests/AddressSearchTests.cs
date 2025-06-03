using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingLocation.AddressControllerTests;

[TestFixture]
public class AddressSearchTests
{
    public string _backLink = Guid.NewGuid().ToString();

    [Test]
    public void AddressSearch_ReturnsExpectedView()
    {
        var sut = new AddressController();
        sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetProviderLocations, _backLink);
        var addressSearch = sut.AddressSearch();
        addressSearch.Result.As<ViewResult>().Should().NotBeNull();
        addressSearch.Result.As<ViewResult>().ViewName.Should().Be(AddressController.ViewPath);
        addressSearch.Result.As<ViewResult>().Model.As<AddressSearchViewModel>().BackLink.Should().Be(_backLink);
    }
}
