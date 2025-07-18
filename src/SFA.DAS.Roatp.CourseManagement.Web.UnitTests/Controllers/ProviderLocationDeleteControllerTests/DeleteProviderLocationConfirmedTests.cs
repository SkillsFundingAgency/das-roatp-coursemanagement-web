using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderLocationDeleteControllerTests;

[TestFixture]
public class DeleteProviderLocationConfirmedTests
{
    public string GetProviderLocationsUrl = Guid.NewGuid().ToString();

    [Test, MoqAutoData]
    public void DeleteProviderLocationConfirmed_TempKeyPresent_ReturnsView(
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn,
        Guid id)
    {
        sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetProviderLocations, GetProviderLocationsUrl);

        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        object isLocationDeleted = true;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationDeletedBannerTempDateKey, out isLocationDeleted));

        var result = sut.DeleteProviderLocationConfirmed(ukprn, id);

        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model as ProviderLocationDeletedConfirmedViewModel;
        model.Should().NotBeNull();
        model!.TrainingVenuesUrl.Should().Be(GetProviderLocationsUrl);
    }

    [Test, MoqAutoData]
    public void DeleteProviderLocationConfirmed_TempKeyAbsent_RedirectsToPageNotFound(
        [Greedy] ProviderLocationDeleteController sut,
        int ukprn,
        Guid id)
    {
        sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.GetProviderLocations, GetProviderLocationsUrl);

        var tempDataMock = new Mock<ITempDataDictionary>();
        sut.TempData = tempDataMock.Object;
        object isLocationDeleted = null;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.ProviderLocationDeletedBannerTempDateKey, out isLocationDeleted));

        var result = sut.DeleteProviderLocationConfirmed(ukprn, id);

        var viewResult = result as RedirectToRouteResult;
        viewResult.Should().NotBeNull();
        viewResult!.RouteName.Should().Be("traininglocations");
    }
}
