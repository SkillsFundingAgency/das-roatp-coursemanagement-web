using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Courses.Queries.GetProviderAvailableCourseDetails;

public class GetProviderAvailableCourseDetailsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task WhenApiCallSucceeds_ThenReturnsResult(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetProviderAvailableCourseDetailsQueryHandler sut,
            GetProviderAvailableCourseDetailsQuery request,
            GetProviderAvailableCourseDetailsQueryResult expectedResult)
    {
        var url = $"providers/{request.Ukprn}/available-courses/{request.CourseType}/{request.LarsCode}";

        apiClientMock
            .Setup(client => client.Get<GetProviderAvailableCourseDetailsQueryResult>(url))
            .ReturnsAsync(expectedResult);

        var actualResult = await sut.Handle(request, CancellationToken.None);

        actualResult.Should().BeEquivalentTo(expectedResult);
    }

    [Test, MoqAutoData]
    public async Task WhenApiCallIsNotSuccessful_ThenReturnsNull(
        [Frozen] Mock<IApiClient> apiClientMock,
        GetProviderAvailableCourseDetailsQueryHandler sut,
        GetProviderAvailableCourseDetailsQuery request)
    {
        var url = $"providers/{request.Ukprn}/available-courses/{request.CourseType}/{request.LarsCode}";

        apiClientMock
            .Setup(client => client.Get<GetProviderAvailableCourseDetailsQueryResult>(url))
            .ReturnsAsync((GetProviderAvailableCourseDetailsQueryResult)null);

        var actualResult = await sut.Handle(request, CancellationToken.None);

        actualResult.Should().BeNull();
    }
}
