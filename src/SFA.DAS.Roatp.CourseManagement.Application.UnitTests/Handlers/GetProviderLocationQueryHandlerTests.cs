using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetProviderLocationQueryHandlerTests
    {
        private GetProviderLocationQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetProviderLocationQueryHandler>> _logger;
        private GetProviderLocationQuery _query;
        private GetProviderLocationQueryResult _queryResult;
        private List<Domain.ApiModels.ProviderLocation> _providerLocations;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetProviderLocationQuery>();
            _queryResult = autoFixture.Create<GetProviderLocationQueryResult>();
            _providerLocations = autoFixture.Create<List<Domain.ApiModels.ProviderLocation>>();
            _queryResult.ProviderLocations = _providerLocations;
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetProviderLocationQueryHandler>>();
        }
        
        [Test]
        public async Task Handle_WithValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.ProviderLocation>>($"/providers/{_query.Ukprn}/locations")).ReturnsAsync(() => _providerLocations);
            _handler = new GetProviderLocationQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.ProviderLocations.Should().BeEquivalentTo(_queryResult.ProviderLocations.FindAll(l=>l.LocationType == Domain.ApiModels.LocationType.Provider));
        }

        [Test]
        public void Handle_NoTrainingLocations_ShouldReturnsException()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.ProviderLocation>>($"/providers/{_query.Ukprn}/locations")).ReturnsAsync(() => new List<Domain.ApiModels.ProviderLocation>());
            _handler = new GetProviderLocationQueryHandler(_apiClient.Object, _logger.Object);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Handle_NoProviderLocations_ShouldReturnsEmptyList()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.ProviderLocation>>($"/providers/{_query.Ukprn}/locations")).ReturnsAsync(() => new List<Domain.ApiModels.ProviderLocation>() {new Domain.ApiModels.ProviderLocation() });
            _handler = new GetProviderLocationQueryHandler(_apiClient.Object, _logger.Object);
            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
            result.ProviderLocations.Count.Should().Be(0);
        }

        [Test]
        public void Handle_Returns_Exception()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.ProviderLocation>>($"/providers/{_query.Ukprn}/locations")).Throws(new Exception());
            _handler = new GetProviderLocationQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
