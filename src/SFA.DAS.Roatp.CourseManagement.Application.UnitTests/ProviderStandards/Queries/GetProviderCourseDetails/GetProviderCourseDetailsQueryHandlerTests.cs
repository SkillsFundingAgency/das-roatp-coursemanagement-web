using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetProviderCourseDetails
{
    public class GetProviderCourseDetailsQueryHandlerTests
    {
        private GetProviderCourseDetailsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private Mock<ILogger<GetProviderCourseDetailsQueryHandler>> _logger;
        private GetProviderCourseDetailsQuery _query;
        private Domain.ApiModels.StandardDetails _standardDetails;

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetProviderCourseDetailsQuery>();
            _standardDetails = autoFixture.Create<Domain.ApiModels.StandardDetails>();
            _apiClient = new Mock<IApiClient>();
            _logger = new Mock<ILogger<GetProviderCourseDetailsQueryHandler>>();
        }

        [Test]
        public async Task ValidRequest_ReturnsValidResponse()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).ReturnsAsync(() => _standardDetails);
            _handler = new GetProviderCourseDetailsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(_standardDetails);
        }

        [Test]
        public async Task NoStandardDetailsReturned_ReturnsNullResponse()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).ReturnsAsync(() => null);

            _handler = new GetProviderCourseDetailsQueryHandler(_apiClient.Object, _logger.Object);

            var result = await _handler.Handle(_query, CancellationToken.None);

            Assert.IsNull(result);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [Test]
        public void Returns_Exception()
        {
            _apiClient.Setup(x => x.Get<Domain.ApiModels.StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).Throws(new Exception());
            _handler = new GetProviderCourseDetailsQueryHandler(_apiClient.Object, _logger.Object);
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}