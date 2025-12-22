using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests
{
    public abstract class EditNationalDeliveryOptionControllerTestBase
    {
        protected const string LarsCode = "123";
        protected Mock<ISessionService> SessionServiceMock;
        protected EditNationalDeliveryOptionController Sut;
        protected Mock<IMediator> MediatorMock;

        public void SetupController()
        {
            MediatorMock = new Mock<IMediator>();
            SessionServiceMock = new Mock<ISessionService>();

            Sut = new EditNationalDeliveryOptionController(MediatorMock.Object, SessionServiceMock.Object, Mock.Of<ILogger<EditNationalDeliveryOptionController>>());

            Sut
                .AddDefaultContextWithUser();
        }

        public void SetLocationOptionInSession(LocationOption locationOption) => SessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption)).Returns(locationOption.ToString());
    }
}
