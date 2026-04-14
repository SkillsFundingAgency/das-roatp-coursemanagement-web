using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerPost_LookupAddressEditTests
{
    [Test, MoqAutoData]
    public void LookupAddressEdit_InvalidStatus_ReturnsViewResult(
       [Greedy] AddTrainingVenueController sut,
       AddTrainingVenueSubmitModel model)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "Test";

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.LookupAddressEdit(model, apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(AddTrainingVenueController.ViewPath);
        var viewModel = viewResult.Model as AddTrainingVenueViewModel;
        viewModel.Route.Should().Be(RouteNames.PostAddTrainingVenueEditShortCourse);
        viewModel.IsAddJourney.Should().Be(false);
        viewModel.SubmitButtonText.Should().Be(ButtonText.Continue);
    }

    [Test, MoqAutoData]
    public void LookupAddressEdit_Valid_SetsSelectedAddressInTempDataAndRedirectsToCorrectRoute(
        [Greedy] AddTrainingVenueController sut,
        AddTrainingVenueSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var larsCode = "Test";

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
        var response = sut.LookupAddressEdit(submitModel, apprenticeshipType, larsCode);

        // Assert
        var result = response as RedirectToRouteResult;
        Assert.IsNotNull(result);
        result.RouteName.Should().Be(RouteNames.GetConfirmAddTrainingVenueEditShortCourse);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, expectedValueInTempData));
    }
}
