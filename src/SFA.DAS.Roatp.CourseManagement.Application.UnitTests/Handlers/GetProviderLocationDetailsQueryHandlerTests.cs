using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetProviderLocationDetailsQueryHandlerTests
    {
        private GetProviderLocationDetailsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetProviderLocationDetailsQuery _query;
        private readonly ProviderLocation _providerLocation = new ProviderLocation() { LocationType = LocationType.Provider };

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetProviderLocationDetailsQuery>();
            _apiClient = new Mock<IApiClient>();
            _handler = new GetProviderLocationDetailsQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderLocationDetailsQueryHandler>>());
        }
        
        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<ProviderLocation>($"providers/{_query.Ukprn}/locations/{_query.Id}")).ReturnsAsync(_providerLocation);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderLocation.Should().BeEquivalentTo(_providerLocation);
        }

        [Test]
        public void Handle_InvalidApiResponse_ThrowsException()
        {
            _apiClient.Setup(x => x.Get<ProviderLocation>($"providers/{_query.Ukprn}/locations/{_query.Id}")).ReturnsAsync((ProviderLocation)null);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
