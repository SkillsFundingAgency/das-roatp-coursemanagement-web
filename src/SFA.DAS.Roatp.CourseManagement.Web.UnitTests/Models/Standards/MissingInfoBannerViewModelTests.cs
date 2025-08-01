using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards;
public class MissingInfoBannerViewModelTests
{
    [TestCase(false, true, true, true)]
    [TestCase(false, true, false, false)]
    [TestCase(true, true, true, false)]
    public void HasMissingLocationIsSet(bool hasLocations, bool isRegulatedForProvider, bool? isApprovedByRegulator, bool expected)
    {
        MissingInfoBannerViewModel sut =
            new MissingInfoBannerViewModel(isRegulatedForProvider, hasLocations, isApprovedByRegulator);

        sut.HasMissingLocation.Should().Be(expected);
    }

    [TestCase(false, true, false, false)]
    [TestCase(true, false, false, false)]
    [TestCase(true, true, true, false)]
    [TestCase(true, true, false, true)]
    public void HasMissingRegulatorApprovalIsSet(bool hasLocations, bool isRegulatedForProvider, bool? isApprovedByRegulator, bool expected)
    {
        MissingInfoBannerViewModel sut =
            new MissingInfoBannerViewModel(isRegulatedForProvider, hasLocations, isApprovedByRegulator);

        sut.HasMissingRegulatorApproval.Should().Be(expected);
    }

    [TestCase(true, true, false, false)]
    [TestCase(true, false, false, false)]
    [TestCase(true, true, true, false)]
    [TestCase(false, true, false, true)]
    public void HasMissingLocationAndRegulatorApprovalIsSet(bool hasLocations, bool isRegulatedForProvider, bool? isApprovedByRegulator, bool expected)
    {
        MissingInfoBannerViewModel sut =
            new MissingInfoBannerViewModel(isRegulatedForProvider, hasLocations, isApprovedByRegulator);

        sut.HasMissingLocationAndRegulatorApproval.Should().Be(expected);
    }
}
