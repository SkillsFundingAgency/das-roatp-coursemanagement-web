using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System.Collections.Generic;
using System.Threading;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests
{
    public abstract class EditLocationOptionControllerTestBase
    {
        protected Mock<ILogger<EditLocationOptionController>> _loggerMock;
        protected Mock<IMediator> _mediatorMock;
        protected Mock<ISessionService> _sessionServiceMock;
        protected EditLocationOptionController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditLocationOptionController>>();
            _mediatorMock = new Mock<IMediator>();
            _sessionServiceMock = new Mock<ISessionService>();

            _sut = new EditLocationOptionController(_mediatorMock.Object, _loggerMock.Object, _sessionServiceMock.Object);
            _sut
                .AddDefaultContextWithUser()
                .AddDefaultUrlMock(RouteNames.ViewStandardDetails);
        }

        protected void SetProviderCourseLocationsInMediatorResponse(List<ProviderCourseLocation> providerCourseLocations) => 
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetStandardDetailsQueryResult { ProviderCourseLocations = providerCourseLocations });
    }
}
