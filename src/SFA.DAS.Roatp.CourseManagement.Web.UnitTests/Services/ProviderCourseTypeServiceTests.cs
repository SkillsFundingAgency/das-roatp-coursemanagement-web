using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderCourseType.Queries.GetProviderCourseType;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Session;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Testing.AutoFixture;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services;
public class ProviderCourseTypeServiceTests
{
    [Test, MoqAutoData]
    public async Task GetProviderCourseType_FromSession(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderCourseTypeService sut,
        ProviderCourseTypeSessionModel providerCourseTypeSessionModel
    )
    {
        var ukprn = 12345;
        sessionServiceMock.Setup(x => x.Get<ProviderCourseTypeSessionModel>()).Returns(providerCourseTypeSessionModel);

        var returnedCourseTypes = await sut.GetProviderCourseType(ukprn);

        returnedCourseTypes.Should().BeEquivalentTo(providerCourseTypeSessionModel.CourseTypes);
        sessionServiceMock.Verify(x => x.Get<ProviderCourseTypeSessionModel>(), Times.Once);
        mediatorMock.Verify(x => x.Send(It.Is<GetProviderCourseTypeQuery>(x => x.Ukprn == ukprn), CancellationToken.None), Times.Never);
        sessionServiceMock.Verify(x => x.Set(It.IsAny<ProviderCourseTypeSessionModel>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task GetProviderCourseType_NotInSession_PutInSessionAndReturned(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] ProviderCourseTypeService sut,
        ProviderCourseTypeSessionModel providerCourseTypeSessionModel,
        List<CourseTypeModel> courseTypeModels
    )
    {
        providerCourseTypeSessionModel.CourseTypes = courseTypeModels;
        var ukprn = 12345;
        sessionServiceMock.Setup(x => x.Get<ProviderCourseTypeSessionModel>()).Returns((ProviderCourseTypeSessionModel)null);
        mediatorMock.Setup(x => x.Send(It.Is<GetProviderCourseTypeQuery>(x => x.Ukprn == ukprn), CancellationToken.None)).ReturnsAsync(courseTypeModels);

        var returnedCourseTypes = await sut.GetProviderCourseType(ukprn);

        returnedCourseTypes.Should().BeEquivalentTo(providerCourseTypeSessionModel.CourseTypes);
        sessionServiceMock.Verify(x => x.Get<ProviderCourseTypeSessionModel>(), Times.Once);
        mediatorMock.Verify(x => x.Send(It.Is<GetProviderCourseTypeQuery>(x => x.Ukprn == ukprn), CancellationToken.None), Times.Once);
        sessionServiceMock.Verify(x => x.Set(It.IsAny<ProviderCourseTypeSessionModel>()), Times.Once);
    }
}
