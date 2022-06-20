using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetStandardDetails.GetStandardDetailsQueryResultTests
{
    [TestFixture]
    public class LocationOptionTests : GetStandardDetailsQueryResultTestsBase
    {
        [Test]
        public void NoLocations_ReturnsNone()
        {
            _sut.ProviderCourseLocations = new List<Domain.ApiModels.ProviderCourseLocation>();

            _sut.LocationOption.Should().Be(LocationOption.None);
        }

        [Test]
        public void ProviderLocationsOnly_ReturnsProvider()
        {
            _sut.ProviderCourseLocations.Remove(_regionalLocation);
            _sut.ProviderCourseLocations.Remove(_nationalLocation);

            _sut.LocationOption.Should().Be(LocationOption.ProviderLocation);
        }

        [Test]
        public void NationalLocationsOnly_ReturnsEmployer()
        {
            _sut.ProviderCourseLocations.Remove(_regionalLocation);
            _sut.ProviderCourseLocations.Remove(_providerLocation);

            _sut.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        [Test]
        public void RegionalLocationsOnly_ReturnsEmployer()
        {
            _sut.ProviderCourseLocations.Remove(_nationalLocation);
            _sut.ProviderCourseLocations.Remove(_providerLocation);

            _sut.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        [Test]
        public void AnyNonProviderLocations_ReturnsEmployer()
        {
            _sut.ProviderCourseLocations.Remove(_providerLocation);

            _sut.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        [Test]
        public void HasProviderAndNationalLocations_ReturnsEmployer()
        {
            _sut.ProviderCourseLocations.Remove(_regionalLocation);

            _sut.LocationOption.Should().Be(LocationOption.Both);
        }

        [Test]
        public void HasProviderAndRegionalLocations_ReturnsEmployer()
        {
            _sut.ProviderCourseLocations.Remove(_nationalLocation);

            _sut.LocationOption.Should().Be(LocationOption.Both);
        }
    }
}
