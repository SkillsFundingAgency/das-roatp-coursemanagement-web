using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAShortCourse.SelectShortCourseControllerTests;
public class SelectShortCourseControllerGetTests
{
    [Test, MoqAutoData]
    public async Task SelectShortCourse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectShortCourseController sut,
        GetAvailableProviderStandardsQueryResult queryResult)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var response = await sut.SelectShortCourse(apprenticeshipType);

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectShortCourseViewModel;
        model.Should().NotBeNull();
        model!.ShortCourses.Should().BeEquivalentTo(queryResult.AvailableCourses, o => o.ExcludingMissingMembers());
        model!.ApprenticeshipType.Should().Be(apprenticeshipType);
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ShortCourse), It.IsAny<CancellationToken>()), Times.Once());
    }
}