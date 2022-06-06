using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardDetailsViewModelTests
    {
        [TestCase("regulator name", true)]
        [TestCase("", false)]
        public void ImplicitOperator_ConvertsFromStandardDetails(string regulatorName, bool isRegulated)
        {
            const string courseName = "course name";
            const string level = "2";
            const string iFateReferenceNumber = "STD_1";
            const string sector = "digital";
            const int larsCode = 133;
            const string version = "3";
            var expectedCourseDisplayName = $"{courseName} (Level {level})";
            const string standardInfoUrl = "http://test.com";
            const string contactUsPhoneNumber = "12345";
            const string contactUsEmail = "me@test.com";
            const string contactUsPageUrl = "http://test.com/contact-us";

            var standardDetails = new StandardDetails
            {
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                IFateReferenceNumber = iFateReferenceNumber,
                Sector = sector,
                Version = version,
                RegulatorName = regulatorName,
                StandardInfoUrl = standardInfoUrl,
                ContactUsEmail = contactUsEmail,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                ContactUsPageUrl = contactUsPageUrl
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.CourseName.Should().Be(courseName);
            viewModel.Level.Should().Be(level);
            viewModel.IFateReferenceNumber.Should().Be(iFateReferenceNumber);
            viewModel.Sector.Should().Be(sector);
            viewModel.LarsCode.Should().Be(larsCode);
            viewModel.Version.Should().Be(version);
            viewModel.RegulatorName.Should().Be(regulatorName);
            viewModel.IsStandardRegulated.Should().Be(isRegulated);
            viewModel.CourseDisplayName.Should().Be(expectedCourseDisplayName);
            viewModel.StandardInfoUrl.Should().Be(standardInfoUrl);
            viewModel.ContactUsEmail.Should().Be(contactUsEmail);
            viewModel.ContactUsPhoneNumber.Should().Be(contactUsPhoneNumber);
            viewModel.ContactUsPageUrl.Should().Be(contactUsPageUrl);
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

            var nationalLocation =    new ProviderCourseLocation
                {
                    LocationName = "Test100",
                    LocationType = LocationType.National
                };

            var providerCourseLocations = new List<ProviderCourseLocation>();

            providerCourseLocations.AddRange(courseLocations);
            providerCourseLocations.AddRange(subregionLocations);
            providerCourseLocations.Add(nationalLocation);

            var standardDetails = new StandardDetails
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            
            viewModel.ProviderCourseLocations.Should().BeEquivalentTo(courseLocations);
            viewModel.SubRegionCourseLocations.Should().BeEquivalentTo(subregionLocations);
            viewModel.NationalCourseLocation.Should().BeEquivalentTo(nationalLocation);
        }

        [Test]
        public void ImplicitOperator_NoCourseLocations_ConvertsLocationSummaryToNoneSet()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>();
            var standardDetails = new StandardDetails
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;

            viewModel.LocationSummary.Should().Be(CourseDeliveryMessageFor.NoneSet);
        }

        [TestCase(LocationType.Provider,CourseDeliveryMessageFor.ProvidersOnly )]
        [TestCase(LocationType.Regional, CourseDeliveryMessageFor.SubregionsOnly)]
        [TestCase(LocationType.National, CourseDeliveryMessageFor.NationalOnly)]
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
            var standardDetails = new StandardDetails
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

            var standardDetails = new StandardDetails
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.LocationSummary.Should().Be(CourseDeliveryMessageFor.ProvidersAndSubregions);
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

            var standardDetails = new StandardDetails
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            viewModel.LocationSummary.Should().Be(CourseDeliveryMessageFor.ProvidersAndNational);
        }


        [Test]
        public void ImplicitOperator_ProviderAndSubregionCourseLocations_RegionsContentAndOrderAsExpected()
        {
            var regionName1 = "region 1";
            var regionName2 = "region 2";
            var regionNameToIgnore = "region name to ignore";
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
                    LocationName = "Test10",
                    LocationType = LocationType.Regional,
                    RegionName = regionName2
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test20",
                    LocationType = LocationType.Regional,
                    RegionName = regionName1
                },
                new ProviderCourseLocation
                {
                    LocationName = "Test21",
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

            var standardDetails = new StandardDetails
            {
                ProviderCourseLocations = providerCourseLocations
            };

            StandardDetailsViewModel viewModel = standardDetails;
            var actualRegions = viewModel.Regions();
            actualRegions.Count.Should().Be(2);
            actualRegions[0].Should().Be(regionName1);
            actualRegions[1].Should().Be(regionName2);
        }
    }
}
