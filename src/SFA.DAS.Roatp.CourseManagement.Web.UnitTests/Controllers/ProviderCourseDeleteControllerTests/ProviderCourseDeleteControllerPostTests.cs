using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseDeleteControllerTests
{
    [TestFixture]
    public class ProviderCourseDeleteControllerPostTests
    {
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseDeleteController _sut;
        string verifyUrl = "http://test";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();

            _sut = new ProviderCourseDeleteController(_mediatorMock.Object, Mock.Of<ILogger<ProviderCourseDeleteController>>());
            _sut.AddDefaultContextWithUser()
               .AddUrlHelperMock()
               .AddUrlForRoute(RouteNames.ViewStandards, verifyUrl);
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToRoute(ConfirmDeleteStandardViewModel model, GetStandardInformationQueryResult queryResult)
        {
            _mediatorMock
               .Setup(m => m.Send(It.Is<GetStandardInformationQuery>(q => q.LarsCode == model.StandardInformation.LarsCode), It.IsAny<CancellationToken>()))
               .ReturnsAsync(queryResult);
            var tempDataMock = new Mock<ITempDataDictionary>();
            _sut.TempData = tempDataMock.Object;

            var result =  await _sut.DeleteProviderCourse(model);

            _mediatorMock.Verify(m => m.Send(It.IsAny<DeleteProviderCourseCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            var actual = (RedirectToRouteResult)result;
            Assert.NotNull(actual);
            actual.RouteName.Should().Be(RouteNames.ViewStandards);
            tempDataMock.Verify(t=>t.Add(TempDataKeys.DeleteProviderCourseTempDataKey,true));
        }
    }
}
