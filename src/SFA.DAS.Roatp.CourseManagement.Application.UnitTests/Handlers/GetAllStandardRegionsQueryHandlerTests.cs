﻿using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllStandardRegions;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Handlers
{
    [TestFixture]
    public class GetAllStandardRegionsQueryHandlerTests
    {
        private GetAllStandardRegionsQueryHandler _handler;
        private Mock<IApiClient> _apiClient;
        private GetAllStandardRegionsQuery _query;
        private readonly Region _regionalLocation = new Region() { Id =1, RegionName="Test", SubregionName="Test" };

        [SetUp]
        public void Setup()
        {
            var autoFixture = new Fixture();

            _query = autoFixture.Create<GetAllStandardRegionsQuery>();
            _apiClient = new Mock<IApiClient>();
            _handler = new GetAllStandardRegionsQueryHandler(_apiClient.Object, Mock.Of<ILogger<GetAllStandardRegionsQueryHandler>>());
        }
        
        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponseNoSelectRegions()
        {
            var regions = new List<Region> { _regionalLocation };
            _apiClient.Setup(x => x.Get<List<Region>>($"lookup/regions")).ReturnsAsync(regions);
            _apiClient.Setup(x => x.Get<StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).ReturnsAsync(new StandardDetails());

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Regions.Should().BeEquivalentTo(regions);
            result.Regions.TrueForAll(a => a.IsSelected).Should().BeFalse();
        }

        [Test]
        public async Task Handle_ValidApiRequest_ReturnsValidResponseWithSelectedRegions()
        {
            var regions = new List<Region> { _regionalLocation };
            _apiClient.Setup(x => x.Get<List<Region>>($"lookup/regions")).ReturnsAsync(regions);
            var standardDetails = new StandardDetails { ProviderCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { LocationName = regions[0].RegionName, LocationType = LocationType.Regional } } };
            _apiClient.Setup(x => x.Get<StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).ReturnsAsync(standardDetails);

            var result = await _handler.Handle(_query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Regions.Should().BeEquivalentTo(regions);
            result.Regions.FindAll(a => a.IsSelected).Count.Should().Be(1);
        }

        [Test]
        public void Handle_InvalidApiResponse_ThrowsException()
        {
            _apiClient.Setup(x => x.Get<List<Region>>($"lookup/regions")).ReturnsAsync((List<Region>)null);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
        }

        [Test]
        public void Handle_NoProviderCourses_ThrowsException()
        {
            var regions = new List<Region> { _regionalLocation };
            _apiClient.Setup(x => x.Get<List<Region>>($"lookup/regions")).ReturnsAsync(regions);
            _apiClient.Setup(x => x.Get<StandardDetails>($"providers/{_query.Ukprn}/courses/{_query.LarsCode}")).ReturnsAsync((StandardDetails)null);

            Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(_query, CancellationToken.None));
        }
    }
}