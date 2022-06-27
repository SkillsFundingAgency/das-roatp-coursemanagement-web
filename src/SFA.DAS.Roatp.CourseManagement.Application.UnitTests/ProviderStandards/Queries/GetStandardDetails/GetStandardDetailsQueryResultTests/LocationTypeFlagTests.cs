using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetStandardDetails.GetStandardDetailsQueryResultTests
{
    [TestFixture]
    public class LocationTypeFlagTests : GetStandardDetailsQueryResultTestsBase
    {
        [Test]
        public void HasProviderLocation_ReturnsTrue()
        {
            _sut.HasProviderLocation.Should().BeTrue();
        }

        [Test]
        public void HasNationalLocation_ReturnsTrue()
        {
            _sut.HasNationalLocation.Should().BeTrue();
        }

        [Test]
        public void HasRegionalLocation_ReturnsTrue()
        {
            _sut.HasRegionalLocation.Should().BeTrue();
        }

        [Test]
        public void HasProviderLocation_ReturnsFalse()
        {
            _sut.ProviderCourseLocations.Remove(_providerLocation);
            _sut.HasProviderLocation.Should().BeFalse();
        }

        [Test]
        public void HasNationalLocation_ReturnsFalse()
        {
            _sut.ProviderCourseLocations.Remove(_nationalLocation);
            _sut.HasNationalLocation.Should().BeFalse();
        }

        [Test]
        public void HasRegionalLocation_ReturnsFalse()
        {
            _sut.ProviderCourseLocations.Remove(_regionalLocation);
            _sut.HasRegionalLocation.Should().BeFalse();
        }

        [Test]
        public void AllLocationTypeFlags_HasNoLocation_ReturnsFalse()
        {
            _sut.ProviderCourseLocations = new List<ProviderCourseLocation>();
            _sut.HasProviderLocation.Should().BeFalse();
            _sut.HasNationalLocation.Should().BeFalse();
            _sut.HasRegionalLocation.Should().BeFalse();
        }
    }
}
