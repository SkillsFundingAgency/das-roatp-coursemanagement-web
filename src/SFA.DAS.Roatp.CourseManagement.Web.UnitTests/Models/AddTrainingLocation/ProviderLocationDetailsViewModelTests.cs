using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddTrainingLocation
{
    [TestFixture]
    public class ProviderLocationDetailsViewModelTests
    {
        [Test, AutoData]
        public void Constructor_BuildsModel(AddressItem addressItem)
        {
            var sut = new ProviderLocationDetailsViewModel(addressItem);
            sut.AddressLine1.Should().Be(addressItem.AddressLine1);
            sut.AddressLine2.Should().Be(addressItem.AddressLine2);
            sut.Town.Should().Be(addressItem.Town);
            sut.Postcode.Should().Be(addressItem.Postcode);

            var expectedAddressDetails = new List<string>();
            if (!string.IsNullOrWhiteSpace(sut.AddressLine1)) expectedAddressDetails.Add(sut.AddressLine1);
            if (!string.IsNullOrWhiteSpace(sut.AddressLine2)) expectedAddressDetails.Add(sut.AddressLine2);
            if (!string.IsNullOrWhiteSpace(sut.Town)) expectedAddressDetails.Add(sut.Town);
            if (!string.IsNullOrWhiteSpace(sut.Postcode)) expectedAddressDetails.Add(sut.Postcode);

            sut.AddressDetails.Should().BeEquivalentTo(expectedAddressDetails);
        }
    }
}
