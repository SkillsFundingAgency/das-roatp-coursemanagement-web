using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetAvailableProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAvailableProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderLocations.Queries.GetAvailableProviderLocations
{
    [TestFixture]
    public class GetAvailableProviderLocationsQueryHandlerTests
    {
        [Test, MoqAutoData]
        public async Task Handler_CallsApiClient_ReturnsResult(
            [Frozen] Mock<IApiClient> apiClientMock,
            GetAvailableProviderLocationsQueryResult expectedResult,
            GetAvailableProviderLocationsQuery request,
            GetAvailableProviderLocationsQueryHandler sut)
        {
            apiClientMock.Setup(a => a.Get<GetAvailableProviderLocationsQueryResult>($"providers/{request.Ukprn}/locations/{request.LarsCode}/available-providerlocations")).ReturnsAsync(expectedResult);
            
            var actualResult = await sut.Handle(request, new CancellationToken());

            apiClientMock.Verify(x => x.Get<GetAvailableProviderLocationsQueryResult>($"providers/{request.Ukprn}/locations/{request.LarsCode}/available-providerlocations"), Times.Once);
            expectedResult.Should().BeEquivalentTo(actualResult);
        }
    }
}
