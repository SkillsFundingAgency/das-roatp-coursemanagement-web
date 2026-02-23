using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ManageShortCoursesControllerTests;
public class ManageShortCoursesControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_CourseTypeReturnsShortCourse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ManageShortCoursesController sut,
        GetAllProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var expectedApprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        mediatorMock.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sut.AddDefaultContextWithUser();

        var selectShortCourseUrl = RouteNames.SelectShortCourse;
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.SelectShortCourse, selectShortCourseUrl);

        // Act
        var result = await sut.Index(expectedApprenticeshipType) as ViewResult;

        // Assert
        result.Should().NotBeNull();
        result!.Model.Should().NotBeNull();
        var model = result!.Model as ManageShortCoursesViewModel;
        model!.AddAShortCourseLink.Should().Be(selectShortCourseUrl);
        model!.ApprenticeshipType.Should().Be(expectedApprenticeshipType);
        model!.ShowShortCourseHeading.Should().BeTrue();
        mediatorMock.Verify(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }
}
