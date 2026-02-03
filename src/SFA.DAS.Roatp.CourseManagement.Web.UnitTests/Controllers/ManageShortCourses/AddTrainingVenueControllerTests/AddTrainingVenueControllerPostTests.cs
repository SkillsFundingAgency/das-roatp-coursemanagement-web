using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Text.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.AddTrainingVenueControllerTests;
public class AddTrainingVenueControllerPostTests
{
    [Test, MoqAutoData]
    public void LookupAddress_InvalidStatus_ReturnsViewResult(
       [Greedy] AddTrainingVenueController sut,
       AddTrainingVenueSubmitModel model)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        sut.ModelState.AddModelError("key", "errorMessage");

        // Act
        var result = sut.LookupAddress(model, courseType);

        // Assert
        var viewResult = result.Result as ViewResult;
        Assert.IsNotNull(viewResult);
        viewResult.ViewName.Should().Be(AddTrainingVenueController.ViewPath);
    }

    [Test, MoqAutoData]
    public void LookupAddress_Valid_SetsSelectedAddressInTempDataAndRedirectsToGetAddTrainingVenue(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] AddTrainingVenueController sut,
        AddTrainingVenueSubmitModel submitModel,
        Mock<ITempDataDictionary> tempDataMock)
    {
        // Arrange
        var courseType = CourseType.ApprenticeshipUnit;

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
        var response = sut.LookupAddress(submitModel, courseType);

        // Assert
        var result = response.Result as RedirectToRouteResult;
        Assert.IsNotNull(result);
        result.RouteName.Should().Be(RouteNames.GetAddTrainingVenue);
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedAddressTempDataKey));
        tempDataMock.Verify(t => t.Add(TempDataKeys.SelectedAddressTempDataKey, expectedValueInTempData));
    }
}
