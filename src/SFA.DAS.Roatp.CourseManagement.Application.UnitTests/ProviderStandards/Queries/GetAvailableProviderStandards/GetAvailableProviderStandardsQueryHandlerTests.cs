using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetAvailableProviderStandards;

[TestFixture]
public class GetAvailableProviderStandardsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handler_CallsApiClient_ReturnsResult(
        [Frozen] Mock<IApiClient> apiClientMock,
        GetAvailableProviderStandardsQueryResult expectedResult,
        GetAvailableProviderStandardsQuery request,
        GetAvailableProviderStandardsQueryHandler sut)
    {
        apiClientMock.Setup(a => a.Get<GetAvailableProviderStandardsQueryResult>($"providers/{request.Ukprn}/available-courses/{request.CourseType}")).ReturnsAsync(expectedResult);

        var actualResult = await sut.Handle(request, new CancellationToken());

        apiClientMock.Verify(x => x.Get<GetAvailableProviderStandardsQueryResult>($"providers/{request.Ukprn}/available-courses/{request.CourseType}"), Times.Once);
        expectedResult.Should().BeEquivalentTo(actualResult);
    }
}