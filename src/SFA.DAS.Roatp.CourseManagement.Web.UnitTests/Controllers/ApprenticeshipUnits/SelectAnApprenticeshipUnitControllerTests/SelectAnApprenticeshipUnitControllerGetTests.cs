using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ApprenticeshipUnits;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAnApprenticeshipUnit;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ApprenticeshipUnits.SelectAnApprenticeshipUnitControllerTests;
public class SelectAnApprenticeshipUnitControllerGetTests
{
    [Test, MoqAutoData]
    public async Task Index_ReturnsView(
        [Frozen] Mock<IMediator> mediatorMock,
        [Greedy] SelectAnApprenticeshipUnitController sut,
        GetAvailableProviderStandardsQueryResult queryResult)
    {
        // Arrange
        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ApprenticeshipUnit), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

        // Act
        var response = await sut.Index();

        // Assert
        var viewResult = response as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as SelectAnApprenticeshipUnitViewModel;
        model.Should().NotBeNull();
        model!.ApprenticeshipUnit.Should().BeEquivalentTo(queryResult.AvailableCourses, o => o.ExcludingMissingMembers());
        mediatorMock.Verify(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.CourseType == CourseType.ApprenticeshipUnit), It.IsAny<CancellationToken>()), Times.Once());
    }
}