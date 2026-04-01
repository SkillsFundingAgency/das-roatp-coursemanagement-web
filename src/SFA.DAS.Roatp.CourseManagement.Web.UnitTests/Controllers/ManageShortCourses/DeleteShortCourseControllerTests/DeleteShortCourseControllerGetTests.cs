using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.DeleteShortCourseControllerTests;
public class DeleteShortCourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task DeleteShortCourse_ValidRequest_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] DeleteShortCourseController sut,
        GetStandardInformationQueryResult queryResult,
        GetProviderCourseDetailsQueryResult getStandardDetailsQueryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string backToManageShortCoursesLink = Guid.NewGuid().ToString();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardDetailsQueryResult);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ManageShortCourses, backToManageShortCoursesLink);

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        var model = viewResult!.Model as DeleteShortCourseViewModel;
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.BackToManageShortCoursesLink.Should().Be(backToManageShortCoursesLink);
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourse_ValidRequest_VerifyMediatorIsInvoked(
       [Frozen] Mock<IMediator> mediatorMock,
       [Greedy] DeleteShortCourseController sut,
       GetStandardInformationQueryResult queryResult,
       GetProviderCourseDetailsQueryResult getStandardDetailsQueryResult,
       string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string backToManageShortCoursesLink = Guid.NewGuid().ToString();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardDetailsQueryResult);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ManageShortCourses, backToManageShortCoursesLink);

        // Act
        await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourse_GetStandardDetailsReturnsNull_RedirectsToPageNotFound(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] DeleteShortCourseController sut,
        GetStandardInformationQueryResult queryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync((GetProviderCourseDetailsQueryResult)null);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourse_GetStandardInformationReturnsNull_RedirectsToPageNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] DeleteShortCourseController sut,
    GetProviderCourseDetailsQueryResult getStandardDetailsQueryResult,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardDetailsQueryResult);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync((GetStandardInformationQueryResult)null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}
