using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetStandardQueryHandlerTests
    {
        private GetStandardQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetStandardQueryHandler>> _logger;
        private GetStandardQuery _query;
        private GetStandardQueryResult _queryResult;
        private List<Domain.ApiModels.Standard> _standards;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetStandardQuery>();
            _queryResult = autoFixture.Create<GetStandardQueryResult>();
            _standards = autoFixture.Create<List<Domain.ApiModels.Standard>>();
            _queryResult.Standards = _standards;
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetStandardQueryHandler>>();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).ReturnsAsync(() => _standards);
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.Standards.Should().BeEquivalentTo(_queryResult.Standards);
        }

        [Test]
        public async Task Handle_NoStandardsReturned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).ReturnsAsync(() => null);
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            Assert.IsNull(result);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public void GetStandardQueryHandler_Returns_Exception()
        {
            _apiClient.Setup(x => x.Get<List<Domain.ApiModels.Standard>>($"/Standards/{_query.Ukprn}")).Throws(new Exception());
            _handler = new GetStandardQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
