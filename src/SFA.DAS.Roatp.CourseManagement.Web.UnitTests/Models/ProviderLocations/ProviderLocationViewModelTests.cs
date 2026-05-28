using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ProviderLocations
{
    [TestFixture]
    public class ProviderLocationViewModelTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConvertsFromProviderLocation(ProviderLocation providerLocation)
        {
            ProviderLocationViewModel sut = providerLocation;

            sut.Should().BeEquivalentTo(providerLocation, o =>
            {
                o.Excluding(c => c.LocationType).Excluding(c => c.Standards);
                return o;
            });

            var sutFirstStandard = sut.Standards.First();
            var providerLocationFirstStandard = providerLocation.Standards.Where(s => s.LearningType == ApprenticeshipType.Apprenticeship).OrderBy(s => s.Title).First();

            sutFirstStandard.Should()
                .BeEquivalentTo(providerLocationFirstStandard,
                    o =>
                        o.Excluding(s => s.StandardUrl)
                        .Excluding(s => s.HasOtherVenues)
                        .Excluding(s => s.CourseDisplayName));
        }

        [Test]
        public void ImplicitOperator_CourseFlagsAreTrue()
        {
            var providerLocation = new ProviderLocation
            {
                Standards = new List<LocationStandardModel>
                {
                    new LocationStandardModel
                    {
                        LearningType = ApprenticeshipType.Apprenticeship,
                        Title = "Standard 1"
                    },
                    new LocationStandardModel
                    {
                        LearningType = ApprenticeshipType.ApprenticeshipUnit,
                        Title = "Apprenticeship Unit 1"
                    }
                }
            };

            ProviderLocationViewModel sut = providerLocation;

            sut.HasCourses.Should().BeTrue();
            sut.ShowStandards.Should().BeTrue();
            sut.ShowApprenticeshipUnits.Should().BeTrue();
        }

        [Test]
        public void ImplicitOperator_CourseFlagsAreFalse()
        {
            var providerLocation = new ProviderLocation();

            ProviderLocationViewModel sut = providerLocation;

            sut.HasCourses.Should().BeFalse();
            sut.ShowStandards.Should().BeFalse();
            sut.ShowApprenticeshipUnits.Should().BeFalse();
        }

        [TestCaseSource(nameof(AddressData))]
        public void Constructor_BuildsAddressDetails(string address1, string address2, string address3, string address4, string expectedAddressDetails)
        {
            ProviderLocationViewModel sut = new ProviderLocationViewModel
            {
                AddressLine1 = address1,
                AddressLine2 = address2,
                Town = address3,
                County = address4
            };

            var concatenatedAddressDetails = string.Join(",", sut.AddressDetails);
            concatenatedAddressDetails.Should().Be(concatenatedAddressDetails);
        }

        private static IEnumerable<TestCaseData> AddressData
        {
            get
            {
                yield return new TestCaseData("address line 1", "addressLine 2", "addressLine3", "AddressLine4", "address line 1, addressLine 2, addressLine3, AddressLine4");
                yield return new TestCaseData("", "addressLine 2", "addressLine3", "AddressLine4", "addressLine 2, addressLine3, AddressLine4");
                yield return new TestCaseData(null, "addressLine 2", "addressLine3", "AddressLine4", "addressLine 2, addressLine3, AddressLine4");
                yield return new TestCaseData("address line 1", "", "addressLine3", "AddressLine4", "address line 1, addressLine3, AddressLine4");
                yield return new TestCaseData("address line 1", null, "addressLine3", "AddressLine4", "address line 1, addressLine3, AddressLine4");
                yield return new TestCaseData("address line 1", "addressLine 2", "", "AddressLine4", "address line 1, addressLine 2, AddressLine4");
                yield return new TestCaseData("address line 1", "addressLine 2", null, "AddressLine4", "address line 1, addressLine 2, AddressLine4");
                yield return new TestCaseData("address line 1", "addressLine 2", "addressLine3", "", "address line 1, addressLine 2, addressLine3");
                yield return new TestCaseData("address line 1", "addressLine 2", "addressLine3", null, "address line 1, addressLine 2, addressLine3");
                yield return new TestCaseData("address line 1", "", "", "", "address line 1");
                yield return new TestCaseData("address line 1", null, null, null, "address line 1");
            }
        }
    }
}
