using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.ManageShortCourseDetailsControllerGetTests;
public class ManageShortCourseDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public async Task ManageShortCourseDetails_GetStandardsDetailsApiReturnedValidResponse_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ManageShortCourseDetailsController sut,
        GetStandardDetailsQueryResult apiResponse)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        int ukprn = 12345;
        string larsCode = "ABC1234";
        string backToManageShortCoursesLink = Guid.NewGuid().ToString();
        string contactDetailsChangeLink = Guid.NewGuid().ToString();
        string deleteShortCourseLink = Guid.NewGuid().ToString();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ProviderClaims.ProviderUkprn, ukprn.ToString()) }, "mock"));

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardDetailsQuery>(r => r.Ukprn == ukprn && r.LarsCode == larsCode), It.IsAny<CancellationToken>())).ReturnsAsync(apiResponse);

        sut.AddDefaultContextWithUser();
        sut.AddUrlHelperMock()
            .AddUrlForRoute(RouteNames.ManageShortCourses, backToManageShortCoursesLink)
            .AddUrlForRoute(RouteNames.EditShortCourseContactDetails, contactDetailsChangeLink)
            .AddUrlForRoute(RouteNames.DeleteShortCourse, deleteShortCourseLink);

        sut.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user },
        };

        // Act
        var result = await sut.ManageShortCourseDetails(apprenticeshipType, larsCode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult!.Model as ManageShortCourseDetailsViewModel;
        model!.Should().NotBeNull();
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.BackToManageShortCoursesLink.Should().Be(backToManageShortCoursesLink);
        model.DeleteShortCourseLink.Should().Be(deleteShortCourseLink);
        model.ContactInformation.ContactDetailsChangeLink.Should().Be(contactDetailsChangeLink);
        model.LocationInformation.TrainingRegionsChangeLink.Should().Be("#");
        model.LocationInformation.TrainingVenuesChangeLink.Should().Be("#");
        model.LocationInformation.NationalProviderChangeLink.Should().Be("#");
        model.LocationInformation.LocationOptionsChangeLink.Should().Be("#");
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardDetailsQuery>(r => r.Ukprn == ukprn && r.LarsCode == larsCode), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test, MoqAutoData]
    public async Task ManageShortCourseDetails_GetStandardsDetailsApiReturnsNull_RedirectsPageNotFounds(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] ManageShortCourseDetailsController sut)
    {
        // Arrange
        string larsCode = "ABC1234";
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        sut.AddDefaultContextWithUser();
        mediatorMock.Setup(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);

        // Act
        var result = await sut.ManageShortCourseDetails(apprenticeshipType, larsCode);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        mediatorMock.Verify(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()), Times.Once());
    }
}
