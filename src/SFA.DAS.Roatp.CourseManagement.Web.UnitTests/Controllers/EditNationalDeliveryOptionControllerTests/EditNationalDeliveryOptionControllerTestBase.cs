using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests
{
    public abstract class EditNationalDeliveryOptionControllerTestBase
    {
        protected const int LarsCode = 123;
        protected static string BackLinkUrl = Guid.NewGuid().ToString();
        protected static string CancelLinkUrl = Guid.NewGuid().ToString();
        protected Mock<ISessionService> SessionServiceMock;
        protected EditNationalDeliveryOptionController Sut;
        protected Mock<IMediator> MediatorMock;

        public void SetupController()
        {
            MediatorMock = new Mock<IMediator>();
            SessionServiceMock = new Mock<ISessionService>();

            Sut = new EditNationalDeliveryOptionController(MediatorMock.Object, SessionServiceMock.Object, Mock.Of<ILogger<EditNationalDeliveryOptionController>>());

            Sut
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetLocationOption, BackLinkUrl)
                .AddUrlForRoute(RouteNames.GetStandardDetails, CancelLinkUrl);
        }

        public void SetLocationOptionInSession(LocationOption locationOption) => SessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption, LarsCode.ToString())).Returns(locationOption.ToString());
    }
}
