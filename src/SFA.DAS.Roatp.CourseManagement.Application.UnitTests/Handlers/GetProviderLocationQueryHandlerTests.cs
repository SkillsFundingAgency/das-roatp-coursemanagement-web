using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetProviderLocationQueryHandlerTests
    {
        private GetProviderLocationQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetProviderLocationQuery _query;
        private readonly ProviderLocation _regionalLocation = new ProviderLocation() { LocationType = LocationType.Regional };
        private readonly ProviderLocation _nationalLocation = new ProviderLocation() { LocationType = LocationType.National };
        private readonly ProviderLocation _providerLocation = new ProviderLocation() { LocationType = LocationType.Provider };

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetProviderLocationQuery>();
            _apiClient = new Mock<IApiClient>();
            _handler = new GetProviderLocationQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderLocationQueryHandler>>());
        }
        
        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponse()
        {
            var providerLocations = new List<ProviderLocation> { _providerLocation };
            _apiClient.Setup(x => x.Get<List<ProviderLocation>>($"providers/{_query.Ukprn}/locations")).ReturnsAsync(providerLocations);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderLocations.Should().BeEquivalentTo(providerLocations);
        }

        [Test]
        public void Handle_InvalidApiResponse_ThrowsException()
        {
            _apiClient.Setup(x => x.Get<List<ProviderLocation>>($"providers/{_query.Ukprn}/locations")).ReturnsAsync((List<ProviderLocation>)null);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
        }

        [Test]
        public async Task Handle_NoProviderTrainingLocations_ReturnsEmptyList()
        {
            _apiClient.Setup(x => x.Get<List<ProviderLocation>>($"providers/{_query.Ukprn}/locations")).ReturnsAsync(() => new List<ProviderLocation>());

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderLocations.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_NoProviderLocations_ShouldReturnsEmptyList()
        {
            var providerLocations = new List<ProviderLocation>() { _regionalLocation, _nationalLocation };
            _apiClient.Setup(x => x.Get<List<ProviderLocation>>($"providers/{_query.Ukprn}/locations")).ReturnsAsync(providerLocations);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderLocations.Should().BeEmpty();
        }


        [Test]
        public async Task Handle_ReturnsProviderTrainingLocationsOnly()
        {
            var providerLocations = new List<ProviderLocation>() { _regionalLocation, _nationalLocation, _providerLocation };
            var expectedProviderLocations = new List<ProviderLocation>() { _providerLocation };
            _apiClient.Setup(x => x.Get<List<ProviderLocation>>($"providers/{_query.Ukprn}/locations")).ReturnsAsync(providerLocations);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderLocations.Should().BeEquivalentTo(expectedProviderLocations);
        }

    }
}
