using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.SelectAStandardControllerTests
{
    [TestFixture]
    public class SelectAStandardControllerGetTests
    {
        [Test, MoqAutoData]
        public async Task SelectAStandard_ReturnsViewResult(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] SelectAStandardController sut,
            GetAvailableProviderStandardsQueryResult queryResult)
        {
            sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.ViewStandards);
            mediatorMock.Setup(m => m.Send(It.Is<GetAvailableProviderStandardsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult);

            var response = await sut.SelectAStandard();

            var viewResult = response as ViewResult;
            Assert.IsNotNull(viewResult);
            viewResult.ViewName.Should().Be(SelectAStandardController.ViewPath);
            var model = viewResult.Model as SelectAStandardViewModel;
            model.CancelLink.Should().Be(TestConstants.DefaultUrl);
            var expectedNames = queryResult.AvailableCourses.Select(s => $"{s.Title} (Level {s.Level})");
            model.Standards.All(s => expectedNames.Contains(s.Text)).Should().BeTrue();
        }
    }
}
