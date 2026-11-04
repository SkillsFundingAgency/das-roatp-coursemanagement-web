using System.Collections.Generic;
using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests;

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
        _validatorMock.Setup(x => x.Validate(It.IsAny<ConfirmNationalProviderSubmitModel>())).Returns(new ValidationResult());

        Sut = new EditNationalDeliveryOptionController(MediatorMock.Object, SessionServiceMock.Object, Mock.Of<ILogger<EditNationalDeliveryOptionController>>(), _validatorMock.Object);

        Sut.AddDefaultContextWithUser();
    }

    public void SetLocationOptionInSession(LocationOption locationOption) => SessionServiceMock.Setup(s => s.Get<EditLocationOptionsSessionModel>()).Returns(new EditLocationOptionsSessionModel() { UpdatedLocationOption = locationOption });

    protected GetProviderCourseDetailsQueryResult SetProviderCourseLocationsInMediatorResponse(List<ProviderCourseLocation> providerCourseLocations)
    {
        var result = new GetProviderCourseDetailsQueryResult { ProviderCourseLocations = providerCourseLocations };
        MediatorMock
            .Setup(m => m.Send(It.IsAny<GetProviderCourseDetailsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return result;
    }
}
