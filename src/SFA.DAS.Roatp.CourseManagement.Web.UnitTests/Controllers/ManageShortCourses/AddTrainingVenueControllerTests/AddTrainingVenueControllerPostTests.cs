using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerPostTests
{
    [Test]
    [MoqInlineAutoData("", true, "Confirm")]
    [MoqInlineAutoData("test", false, "Continue")]
    public void LookupAddress_InvalidStatus_ReturnsViewResult(
       string larsCode,
       bool hasSeenSummaryPage,
       string expectedSubmitButtonText,
       [Frozen] Mock<ISessionService> sessionServiceMock,
       [Greedy] AddTrainingVenueController sut,
       AddTrainingVenueSubmitModel model,
       ShortCourseSessionModel sessionModel)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sessionModel.HasSeenSummaryPage = hasSeenSummaryPage;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.LookupAddress(model, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result.Result as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(AddTrainingVenueController.ViewPath);
        var viewModel = viewResult.Model as AddTrainingVenueViewModel;
        viewModel.SubmitButtonText.Should().Be(expectedSubmitButtonText);
    }

    [Test]
    [MoqInlineAutoData("")]
    [MoqInlineAutoData("test")]
    public void LookupAddress_Valid_SetsSelectedAddressInTempDataAndRedirectsToGetAddTrainingVenue(
        string larsCode,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut,
        AddTrainingVenueSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        AddressItem selectedAddress = new AddressItem
        {
            AddressLine1 = submitModel.AddressLine1,
            AddressLine2 = submitModel.AddressLine2,
            Town = submitModel.Town,
            County = submitModel.County,
            Latitude = submitModel.Latitude,
            Longitude = submitModel.Longitude,
            Postcode = submitModel.Postcode,
            Uprn = null
        };

        var expectedValueInTempData = JsonSerializer.Serialize(selectedAddress);
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;

        // Act
        var response = sut.LookupAddress(submitModel, apprenticeshipType, larsCode);

        // Assert
        var result = response.Result as RedirectToRouteResult;
        Assert.IsNotNull(result);
        result.RouteName.Should().Be(RouteNames.GetConfirmAddTrainingVenue);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, expectedValueInTempData));
    }

    [Test, MoqAutoData]
    public async Task LookupAddress_IsAddJourney_SessionIsNull_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut,
        AddTrainingVenueSubmitModel submitMode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.TempData = tempDataMock.Object;

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns((ShortCourseSessionModel)null);

        // Act
        var result = await sut.LookupAddress(submitMode, apprenticeshipType, "") as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        result!.RouteName.Should().Be(RouteNames.ReviewYourDetails);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }
}
