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
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ConfirmAddProviderLocationControllerTests.ConfirmLocationTests;
public class WhenInShortCourseJourney_AndGettingAddProviderLocationCancellation
{
    private ApprenticeshipType _learningType;

    [SetUp]
    public void Before_Each_Test()
    {
        _learningType = ApprenticeshipType.ApprenticeshipUnit;
    }

    [Test, MoqAutoData]
    public void Then_RemovesTempData(
    Mock<ITempDataDictionary> tempDataMock,
    [Greedy] ConfirmAddProviderLocationController sut,
    AddressItem addressItem)
    {
        // Arrange
        var ukprn = 12345678;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        sut.CancelAddProviderLocation(ukprn, _learningType);

        // Assert
        tempDataMock.Verify(t => t.Remove(TempDataKeys.SelectedTrainingVenueAddressTempDataKey));
    }

    [Test, MoqAutoData]
    public void Then_RedirectsToReviewYourDetails(
        Mock<ITempDataDictionary> tempDataMock,
        [Greedy] ConfirmAddProviderLocationController sut,
        AddressItem addressItem)
    {
        // Arrange
        var ukprn = 12345678;
        sut.AddDefaultContextWithUser();
        sut.TempData = tempDataMock.Object;
        object serialisedAddressItem = JsonSerializer.Serialize(addressItem);
        tempDataMock.Setup(t => t.TryGetValue(TempDataKeys.SelectedTrainingVenueAddressTempDataKey, out serialisedAddressItem));

        // Act
        var result = sut.CancelAddProviderLocation(ukprn, _learningType) as RedirectToRouteResult;

        // Assert
        result!.RouteName.Should().Be(RouteNames.SelectShortCourseLocationOption);
    }
}
