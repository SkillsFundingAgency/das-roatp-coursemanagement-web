using System.Threading;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
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
        protected Mock<IValidator<ConfirmNationalProviderSubmitModel>> _validatorMock;

        public void SetupController()
        {
            MediatorMock = new Mock<IMediator>();
            SessionServiceMock = new Mock<ISessionService>();
            _validatorMock = new Mock<IValidator<ConfirmNationalProviderSubmitModel>>();

            Sut = new EditNationalDeliveryOptionController(MediatorMock.Object, SessionServiceMock.Object, Mock.Of<ILogger<EditNationalDeliveryOptionController>>(), _validatorMock.Object);

            Sut
                .AddDefaultContextWithUser();
        }

        public void SetLocationOptionInSession(LocationOption locationOption) => SessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption)).Returns(locationOption.ToString());

        public void SetUpCorrectCourseTypeGetProviderCourseDetailsApiResponse()
        {
            MediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetProviderCourseDetailsQueryResult()
            {
                CourseType = CourseType.Apprenticeship
            });
        }

        public void SetUpIncorrectCourseTypeGetProviderCourseDetailsApiResponse()
        {
            MediatorMock.Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GetProviderCourseDetailsQueryResult()
            {
                CourseType = CourseType.ShortCourse
            });
        }
    }
}
