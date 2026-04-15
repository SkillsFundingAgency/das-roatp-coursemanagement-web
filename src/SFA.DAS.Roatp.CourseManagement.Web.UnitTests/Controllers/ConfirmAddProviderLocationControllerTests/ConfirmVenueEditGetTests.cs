using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests;
public class ConfirmVenueEditGetTests
{
    [Test, MoqAutoData]
    public void ConfirmVenueEdit_AddressInTempData_ReturnsViewResult(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        Mock<ITempDataDictionary> tempDataMock,
        Mock<IUrlHelper> urlHelperMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        ShortCourseSessionModel sessionModel,
        AddressItem addressItem,
        string cancelLinkUrl)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "test";
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));
        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.CancelAddProviderLocation, cancelLinkUrl);

        // Act
        var result = sut.ConfirmLocationEdit(apprenticeshipType, larsCode) as ViewResult;

        // Assert
        Assert.IsNotNull(result);
        result.ViewName.Should().Be(ConfirmAddProviderLocationController.ViewPath);
        var model = result.Model as ConfirmAddProviderLocationViewModel;
        model!.AddressLine1.Should().Be(addressItem.AddressLine1);
        model!.CancelLink.Should().Be(cancelLinkUrl);
        model!.Route.Should().Be(RouteNames.PostConfirmAddProviderLocationEditCourse);
        model!.ShowCancelOption.Should().BeFalse();
        model!.IsAddJourney.Should().Be(false);
    }

    [Test, MoqAutoData]
    public void ConfirmVenueEdit_AddressNotInTempData_RedirectsToEditShortCourseTrainingVenues(
        string larsCode,
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        object address = null;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out address));

        // Act
        var result = sut.ConfirmLocationEdit(apprenticeshipType, "test") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.EditShortCourseTrainingVenues);
    }
}
