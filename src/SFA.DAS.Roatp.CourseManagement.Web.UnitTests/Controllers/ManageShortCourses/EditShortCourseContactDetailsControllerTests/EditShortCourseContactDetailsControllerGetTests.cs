using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseContactDetailsControllerTests;
public class EditShortCourseContactDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_GetStardardsReturnsData_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        GetProviderCourseDetailsQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult.Model as ShortCourseContactDetailsViewModel;
        model.Should().NotBeNull();
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseContactDetails);
        model.IsAddJourney.Should().BeFalse();
        model.ShowSavedContactDetailsText.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_GetStardardsReturnsData_VerifyMediatorIsInvoked(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseContactDetailsController sut,
    GetProviderCourseDetailsQueryResult queryResult,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseContactDetails_GetStardardsReturnsNull_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] EditShortCourseContactDetailsController sut,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn == int.Parse(TestConstants.DefaultUkprn) && q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.EditShortCourseContactDetails(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}
