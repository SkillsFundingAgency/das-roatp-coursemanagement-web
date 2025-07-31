using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardDetailsViewModelTests
    {
        [Test]
        public void ImplicitOperator_ConvertsFromStandardDetails()
        {
            const string regulatorName = "regulator name";
            const bool isRegulated = true;
            const string courseName = "course name";
            const int level = 2;
            const string iFateReferenceNumber = "STD_1";
            const string sector = "digital";
            const int larsCode = 133;
            const ApprenticeshipType ApprenticeshipType = ApprenticeshipType.FoundationApprenticeship;
            var expectedCourseDisplayName = $"{courseName} (level {level})";
            const string standardInfoUrl = "http://test.com";
            const string contactUsPhoneNumber = "12345";
            const string contactUsEmail = "me@test.com";
            const string contactUsPageUrl = "http://test.com/contact-us";

            var standardDetails = new GetStandardDetailsQueryResult
            {
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                IFateReferenceNumber = iFateReferenceNumber,
                Sector = sector,
                ApprenticeshipType = ApprenticeshipType,
                RegulatorName = regulatorName,
                StandardInfoUrl = standardInfoUrl,
                ContactUsEmail = contactUsEmail,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                ContactUsPageUrl = contactUsPageUrl,
                IsRegulatedForProvider = isRegulated
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.StandardInformation.CourseName.Should().Be(courseName);
            viewModel.StandardInformation.Level.Should().Be(level);
            viewModel.StandardInformation.IfateReferenceNumber.Should().Be(iFateReferenceNumber);
            viewModel.StandardInformation.Sector.Should().Be(sector);
            viewModel.StandardInformation.LarsCode.Should().Be(larsCode);
            viewModel.StandardInformation.ApprenticeshipType.Should().Be(ApprenticeshipType);
            viewModel.StandardInformation.RegulatorName.Should().Be(regulatorName);
            viewModel.StandardInformation.CourseDisplayName.Should().Be(expectedCourseDisplayName);
            viewModel.ContactInformation.StandardInfoUrl.Should().Be(standardInfoUrl);
            viewModel.ContactInformation.ContactUsEmail.Should().Be(contactUsEmail);
            viewModel.ContactInformation.ContactUsPhoneNumber.Should().Be(contactUsPhoneNumber);
            viewModel.StandardInformation.IsRegulatedForProvider.Should().Be(isRegulated);
        }

        [Test]
        public void ImplicitOperator_ConvertsCourseLocationsFromStandardDetails()
        {
            var courseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test",
                    LocationType = LocationType.Provider
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test2",
                    LocationType = LocationType.Provider
                }
            };

            var subregionLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test10",
                    LocationType = LocationType.Regional
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test11",
                    LocationType = LocationType.Regional
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test12",
                    LocationType = LocationType.Regional
                }
            };

            var nationalLocation = new ProviderCourseLocation
            {
                LocationName = "Test100",
                LocationType = LocationType.National
            };

            var providerCourseLocations = new List<ProviderCourseLocation>();

            providerCourseLocations.AddRange(courseLocations);
            providerCourseLocations.AddRange(subregionLocations);
            providerCourseLocations.Add(nationalLocation);

            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;

            viewModel.ProviderCourseLocations.Should().BeEquivalentTo(courseLocations, option => option.Excluding(a => a.HasBlockReleaseDeliveryOption).Excluding(a => a.HasDayReleaseDeliveryOption));
            viewModel.SubRegionCourseLocations.Should().BeEquivalentTo(subregionLocations, option => option.Excluding(a => a.HasBlockReleaseDeliveryOption).Excluding(a => a.HasDayReleaseDeliveryOption));
            viewModel.NationalCourseLocation.Should().BeEquivalentTo(nationalLocation, option => option.Excluding(a => a.HasBlockReleaseDeliveryOption).Excluding(a => a.HasDayReleaseDeliveryOption));
        }

        [Test]
        public void ImplicitOperator_NoCourseLocations_ConvertsLocationSummaryToNoneSet()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>();
            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;

            viewModel.LocationSummary.Should().Be(LocationSummaryCalculator.NoneSet);
        }

        [TestCase(LocationType.Provider, LocationSummaryCalculator.ProvidersOnly)]
        [TestCase(LocationType.Regional, LocationSummaryCalculator.SubregionsOnly)]
        [TestCase(LocationType.National, LocationSummaryCalculator.NationalOnly)]
        public void ImplicitOperator_OnlyOneTypeOfCourseLocations_LocationSummaryAsExpected(LocationType locationType, string whereIsCourseDelivered)
        {
            var courseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test",
                    LocationType = locationType
                }
            };
            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = courseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.LocationSummary.Should().Be(whereIsCourseDelivered);
        }

        [Test]
        public void ImplicitOperator_ProviderAndSubregionCourseLocations_LocationSummaryAsExpected()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test",
                    LocationType = LocationType.Provider
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test10",
                    LocationType = LocationType.Regional
                }
            };

            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.LocationSummary.Should().Be(LocationSummaryCalculator.ProvidersAndSubregions);
        }

        [Test]
        public void ImplicitOperator_ProviderAndNationalCourseLocations_LocationSummaryAsExpected()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test",
                    LocationType = LocationType.Provider
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test10",
                    LocationType = LocationType.National
                }
            };

            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.LocationSummary.Should().Be(LocationSummaryCalculator.ProvidersAndNational);
        }


        [Test]
        public void ImplicitOperator_ProviderAndSubregionCourseLocations_RegionsContentAndOrderAsExpected()
        {
            var regionName1 = "region 1";
            var regionName2 = "region 2";
            var regionNameToIgnore = "region name to ignore";
            var region1_location1 = "region 1 location 1";
            var region2_location1 = "region 2 location 1";
            var region2_location2 = "region 2 location 2";
            var providerCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation
                {
                    LocationName = "Test",
                    LocationType = LocationType.Provider,
                    RegionName = regionNameToIgnore
                },
                new ProviderCourseLocation
                {
                    LocationName = region2_location1,
                    LocationType = LocationType.Regional,
                    RegionName = regionName2
                },
                new ProviderCourseLocation
                {
                    LocationName = region1_location1,
                    LocationType = LocationType.Regional,
                    RegionName = regionName1
                },
                new ProviderCourseLocation
                {
                    LocationName =   region2_location2,
                    LocationType = LocationType.Regional,
                    RegionName = regionName2
                },
                new ProviderCourseLocation
                {
                    LocationName = "National",
                    LocationType = LocationType.National,
                    RegionName = regionNameToIgnore
                }
            };

            var standardDetails = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            var actualRegions = viewModel.Regions();

            var regions = actualRegions.ToList();
            regions.Count.Should().Be(2);
            regions[0].Key.Should().Be(regionName1);
            regions[1].Key.Should().Be(regionName2);

            var locationsInRegion1 = regions[0].ToList();
            var locationsInRegion2 = regions[1].ToList();

            locationsInRegion1[0].LocationName.Should().Be(region1_location1);
            locationsInRegion2[0].LocationName.Should().Be(region2_location1);
            locationsInRegion2[1].LocationName.Should().Be(region2_location2);
        }

        [TestCase(true, "Yes")]
        [TestCase(false, "No")]
        [TestCase(null, "Unknown")]
        public void ImplicitOperator_ConvertsFromStandardDetails(bool? isApprovedByRegulator, string approvedByRegulatorStatus)
        {
            const string regulatorName = "Test regulator";
            var standardDetails = new GetStandardDetailsQueryResult
            {
                RegulatorName = regulatorName,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.StandardInformation.RegulatorName.Should().Be(regulatorName);
            viewModel.IsApprovedByRegulator.Should().Be(isApprovedByRegulator);
            viewModel.ApprovedByRegulatorStatus().Should().Be(approvedByRegulatorStatus);
        }

        [TestCase(false, false, false, true)]
        [TestCase(true, false, null, false)]
        [TestCase(true, true, false, true)]
        [TestCase(true, true, true, false)]
        [TestCase(true, false, null, false)]
        public void StandardRequiresMoreInfoIsSet(bool hasLocations, bool isRegulatedForProvider, bool? isApprovedByRegulator, bool expected)
        {
            var standardDetails = new GetStandardDetailsQueryResult
            {
                HasLocations = hasLocations,
                IsRegulatedForProvider = isRegulatedForProvider,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.StandardRequiresMoreInfo.Should().Be(expected);
        }

        [TestCase(true, true, false, MissingInfoBannerViewModel.MissingInfo.NotApproved)]
        [TestCase(false, false, null, MissingInfoBannerViewModel.MissingInfo.LocationMissing)]
        [TestCase(false, true, false, MissingInfoBannerViewModel.MissingInfo.LocationMissingAndNotApproved)]
        [TestCase(true, true, true, null)]
        public void MissingInformationTypeIsSet(bool hasLocations, bool isRegulatedForProvider,
            bool? isApprovedByRegulator, MissingInfoBannerViewModel.MissingInfo? expected)
        {
            var standardDetails = new GetStandardDetailsQueryResult
            {
                HasLocations = hasLocations,
                IsRegulatedForProvider = isRegulatedForProvider,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.MissingInfoBannerViewModel.MissingInformationType.Should().Be(expected);
        }
    }
}