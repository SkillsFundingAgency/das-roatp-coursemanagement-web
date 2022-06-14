using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetStandardQueryHandlerTests
    {
        private GetAllProviderStandardsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetAllProviderStandardsQueryHandler>> _logger;
        private GetAllProviderStandardsQuery _query;
        private GetAllProviderStandardsQueryResult _queryResult;
        private List<Domain.ApiModels.Standard> _standards;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetAllProviderStandardsQuery>();
            _queryResult = autoFixture.Create<GetAllProviderStandardsQueryResult>();
            _standards = autoFixture.Create<List<Domain.ApiModels.Standard>>();
            _queryResult.Standards = _standards;
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetAllProviderStandardsQueryHandler>>();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"Standards/{_query.Ukprn}")).ReturnsAsync(() => _standards);
            _handler = new GetAllProviderStandardsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.Standards.Should().BeEquivalentTo(_queryResult.Standards);
        }

        [Test]
        public async Task Handle_NoStandardsReturned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"Standards/{_query.Ukprn}")).ReturnsAsync(() => null);
            _handler = new GetAllProviderStandardsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            Assert.IsNull(result);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetStandardQueryHandler_Returns_Exception()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"Standards/{_query.Ukprn}")).Throws(new Exception());
            _handler = new GetAllProviderStandardsQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
