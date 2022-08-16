using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Services
{
    [TestFixture]
    public class LocationSummaryCalculatorTests
    {
        [Test]
        public void GetLocationSummary_NationalAndRegionalAreTrue_ThrowsArgumentException()
        {
            Func<string> action = () => LocationSummaryCalculator.GetLocationSummary(true, true, true);
            action.Should().Throw<ArgumentException>();
        }

        [TestCase(true, true, false, LocationSummaryCalculator.ProvidersAndNational)]
        [TestCase(true, false, true, LocationSummaryCalculator.ProvidersAndSubregions)]
        [TestCase(true, false, false, LocationSummaryCalculator.ProvidersOnly)]
        [TestCase(false, true, false, LocationSummaryCalculator.NationalOnly)]
        [TestCase(false, false, true, LocationSummaryCalculator.SubregionsOnly)]
        [TestCase(false, false, false, LocationSummaryCalculator.NoneSet)]
        public void GetLocationSummary_ReturnApproriateSummary(bool hasProviderLocation, bool hasNationalLocation, bool hasRegionalLocation, string expectedSummary)
        {
            var actualSummary = LocationSummaryCalculator.GetLocationSummary(hasNationalLocation, hasProviderLocation, hasRegionalLocation);
            actualSummary.Should().Be(expectedSummary);
        }
    }
}
