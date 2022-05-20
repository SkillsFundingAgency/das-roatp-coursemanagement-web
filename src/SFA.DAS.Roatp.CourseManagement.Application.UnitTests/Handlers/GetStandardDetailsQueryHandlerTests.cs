using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    public class GetStandardDetailsQueryHandlerTests
    {
        private GetStandardDetailsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetStandardDetailsQueryHandler>> _logger;
        private GetStandardDetailsQuery _query;
        private GetStandardDetailsQueryResult _queryResult;
        private Domain.ApiModels.StandardDetails _standardDetails;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetStandardDetailsQuery>();
            _queryResult = autoFixture.Create<GetStandardDetailsQueryResult>();
            _standardDetails = autoFixture.Create<Domain.ApiModels.StandardDetails>();
            _queryResult.StandardDetails = _standardDetails;
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetStandardDetailsQueryHandler>>();
        }

        [Test]
        public async Task ValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"ProviderCourse/{_query.Ukprn}/Course/{_query.LarsCode}")).ReturnsAsync(() => _standardDetails);
            _handler = new GetStandardDetailsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.StandardDetails.Should().BeEquivalentTo(_queryResult.StandardDetails);
        }

        [Test]
        public async Task NoStandardDetailsReturned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"ProviderCourse/{_query.Ukprn}/Course/{_query.LarsCode}")).ReturnsAsync(() => null);

            _handler = new GetStandardDetailsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            Assert.IsNull(result);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public void Returns_Exception()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"ProviderCourse/{_query.Ukprn}/Course/{_query.LarsCode}")).Throws(new Exception());
            _handler = new GetStandardDetailsQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}
