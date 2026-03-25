using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.DeleteShortCourseControllerTests;
public class DeleteShortCourseControllerPostTests
{
    [Test, MoqAutoData]
    public async Task DeleteShortCourse_InvokesMediatorWithCommandAndRedirectToManageShortCourses(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ITempDataDictionary> tempDataMock,
        [Greedy] DeleteShortCourseController sut,
        DeleteShortCourseSubmitModel model)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;
        sut.TempData = tempDataMock.Object;

        sut.AddDefaultContextWithUser();

        // Act
        var result = await sut.DeleteShortCourse(apprenticeshipType, model);

        // Assert
        var redirectResult = (RedirectToRouteResult)result;
        Assert.NotNull(redirectResult);
        redirectResult.RouteName.Should().Be(RouteNames.ManageShortCourses);
        mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        tempDataMock.Verify(t => t.Add(TempDataKeys.ShowShortCourseDeletedBannerTempDataKey, true));
    }
}
