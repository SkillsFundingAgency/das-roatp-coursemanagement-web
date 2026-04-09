using System.Text.Json;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ConfirmAddTrainingVenueControllerTests;
public class ConfirmAddTrainingVenueControllerGet_CancelAddTrainingVenueTests
{
    [Test, MoqAutoData]
    public void CancelAddTrainingVenue_RemovesTempData_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddTrainingVenueController sut,
        AddressItem addressItem)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        var ukprn = 12345;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.CancelAddTrainingVenue(ukprn, apprenticeshipType) as RedirectToRouteResult;

        // Assert
        result.Should().NotBeNull();
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
    }
}