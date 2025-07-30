using System.Collections.Generic;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models;

[TestFixture]
public class ProviderLocationNotDeletedView()
{
    [Test, AutoData]
    public void ImplicitOperator_ReturnsExpectedLocationName(ProviderLocation providerLocation)
    {
        var sut = (ProviderLocationNotDeletedViewModel)providerLocation;
        sut.LocationName.Should().Be(providerLocation.LocationName);
        sut.BackUrl.Should().BeNull();
    }

    [Test, AutoData]
    public void ImplicitOperator_EmptyStandards_ReturnsEmptyStandardList(ProviderLocation providerLocation)
    {
        providerLocation.Standards = [];
        var sut = (ProviderLocationNotDeletedViewModel)providerLocation;
        sut.StandardsWithoutOtherVenues.Should().BeEquivalentTo(new List<LocationStandardModel>());
    }

    [Test, AutoData]
    public void ImplicitOperator_Standards_ReturnsOrderedStandardsWithNoOtherVenuesOnly(ProviderLocation providerLocation)
    {
        var standardNoOtherVenuesLast = new LocationStandardModel { Title = "ZXY", HasOtherVenues = false };
        var standardNoOtherVenuesFirst = new LocationStandardModel { Title = "ABC", HasOtherVenues = false };
        var standardNoOtherVenuesMiddle = new LocationStandardModel { Title = "MNO", HasOtherVenues = false };
        var standardOtherVenuesFirst = new LocationStandardModel { Title = "DEF", HasOtherVenues = true };
        var standardOtherVenuesSecond = new LocationStandardModel { Title = "PQR", HasOtherVenues = true };

        var setupStandards = new List<LocationStandardModel>
        {
            standardNoOtherVenuesLast,
            standardNoOtherVenuesFirst,
            standardOtherVenuesFirst,
            standardOtherVenuesSecond,
            standardNoOtherVenuesMiddle
        };

        var expectedStandardList = new List<LocationStandardModel>
        {
            standardNoOtherVenuesFirst,
            standardNoOtherVenuesMiddle,
            standardNoOtherVenuesLast
        };

        providerLocation.Standards = setupStandards;
        var sut = (ProviderLocationNotDeletedViewModel)providerLocation;
        sut.StandardsWithoutOtherVenues.Should().BeEquivalentTo(expectedStandardList);
    }
}
