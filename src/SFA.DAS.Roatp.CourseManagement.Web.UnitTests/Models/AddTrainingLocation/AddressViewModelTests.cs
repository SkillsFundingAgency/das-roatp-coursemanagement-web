using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddTrainingLocation
{
    [TestFixture]
    public class AddressViewModelTests
    {
        [Test, AutoData]
        public void AddressHint_HasAddresses_ShowsAddressCount(AddressViewModel sut)
        {
            sut.AddressesHint.Should().Be($"{sut.Addresses.Count} addresses found");
        }

        [Test, AutoData]
        public void AddressHint_NoAddresses_ShowsNoAddressFound(AddressViewModel sut)
        {
            sut.Addresses = new List<SelectListItem>();
            sut.AddressesHint.Should().Be("No address found");
        }

    }
}
