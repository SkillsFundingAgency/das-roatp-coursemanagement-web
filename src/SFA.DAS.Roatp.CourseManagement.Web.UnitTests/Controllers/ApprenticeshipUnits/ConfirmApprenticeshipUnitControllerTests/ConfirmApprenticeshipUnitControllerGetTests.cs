using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ApprenticeshipUnits.ConfirmApprenticeshipUnitControllerTests;
public class ConfirmApprenticeshipUnitControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ConfirmApprenticeshipUnitController sut,
        GetStandardInformationQueryResult queryResult,
        ShortCourseSessionModel sessionModel)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        sessionServiceMock.Setup(s => s.Get<ShortCourseSessionModel>()).Returns(sessionModel);

        // Act
        var response = await sut.Index();

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as ConfirmApprenticeshipUnitViewModel;
        model.Should().NotBeNull();
        model!.ShortCourseInformation.Should().BeEquivalentTo(queryResult, o => o.ExcludingMissingMembers());
        mediatorMock.Verify(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == sessionModel.LarsCode), It.IsAny<CancellationToken>()), Times.Once);
        sessionServiceMock.Verify(s => s.Get<ShortCourseSessionModel>(), Times.Once);
    }
}