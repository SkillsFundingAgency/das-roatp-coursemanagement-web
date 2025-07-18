using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
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
            var providerLocationFirstStandard = providerLocation.Standards.OrderBy(s => s.Title).First();

            sutFirstStandard.Should()
                .BeEquivalentTo(providerLocationFirstStandard,
                    o =>
                        o.Excluding(s => s.StandardUrl)
                        .Excluding(s => s.HasOtherVenues));
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
