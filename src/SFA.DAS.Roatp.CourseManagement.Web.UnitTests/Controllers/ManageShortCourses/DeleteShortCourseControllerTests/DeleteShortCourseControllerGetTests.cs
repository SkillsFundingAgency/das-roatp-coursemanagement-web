using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
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
        GetStandardDetailsQueryResult getStandardDetailsQueryResult,
        string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        string backToManageShortCoursesLink = Guid.NewGuid().ToString();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardDetailsQueryResult);

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
        model.Should().NotBeNull();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.BackToManageShortCoursesLink.Should().Be(backToManageShortCoursesLink);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
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

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync((GetStandardDetailsQueryResult)null);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task DeleteShortCourse_GetStandardInformationReturnsNull_RedirectsToPageNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] DeleteShortCourseController sut,
    GetStandardDetailsQueryResult getStandardDetailsQueryResult,
    string larsCode)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(getStandardDetailsQueryResult);

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync((GetStandardInformationQueryResult)null);

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once);
    }
}
