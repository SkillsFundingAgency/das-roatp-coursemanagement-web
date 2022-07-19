using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetProviderCourseLocationsQueryHandlerTests
    {
        private GetProviderCourseLocationsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetProviderCourseLocationsQuery _query;
        private readonly ProviderCourseLocation _providerCourseLocation = new ProviderCourseLocation() { LocationType = LocationType.Provider };

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetProviderCourseLocationsQuery>();
            _apiClient = new Mock<IApiClient>();
            _handler = new GetProviderCourseLocationsQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetProviderCourseLocationsQueryHandler>>());
        }
        
        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponse()
        {
            var providerCourseLocations = new GetProviderCourseLocationsQueryResult { ProviderCourseLocations = new List<ProviderCourseLocation> { _providerCourseLocation } };
            _apiClient.Setup(x => x.Get<GetProviderCourseLocationsQueryResult>($"providers/{ _query.Ukprn}/courses/{ _query.LarsCode}/locations/provider-locations")).ReturnsAsync(providerCourseLocations);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderCourseLocations.Should().BeEquivalentTo(providerCourseLocations.ProviderCourseLocations);
        }

        [Test]
        public async Task Handle_NullApiResponse_ReturnsEmptyResponse()
        {
            _apiClient.Setup(x => x.Get<GetProviderCourseLocationsQueryResult>($"providers/{ _query.Ukprn}/courses/{ _query.LarsCode}/locations/provider-locations")).ReturnsAsync((GetProviderCourseLocationsQueryResult)null);

            var result = await _handler.Handle(_query, CancellationToken.None);
            result.Should().NotBeNull();
            result.ProviderCourseLocations.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_NoProviderTrainingLocations_ReturnsEmptyList()
        {
            _apiClient.Setup(x => x.Get<GetProviderCourseLocationsQueryResult>($"providers/{ _query.Ukprn}/courses/{ _query.LarsCode}/locations/provider-locations")).ReturnsAsync(() => new GetProviderCourseLocationsQueryResult());

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderCourseLocations.Should().BeEmpty();
        }

        [Test]
        public async Task Handle_NoProviderCourseLocations_ShouldReturnsEmptyList()
        {
            var getProviderCourseLocationsQueryResult = new GetProviderCourseLocationsQueryResult();
            _apiClient.Setup(x => x.Get<GetProviderCourseLocationsQueryResult>($"providers/{ _query.Ukprn}/courses/{ _query.LarsCode}/locations/provider-locations")).ReturnsAsync(getProviderCourseLocationsQueryResult);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.ProviderCourseLocations.Should().BeEmpty();
        }
    }
}
