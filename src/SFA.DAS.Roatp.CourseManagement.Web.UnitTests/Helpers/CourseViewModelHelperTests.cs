using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Helpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Helpers;

public class CourseViewModelHelperTests
{
    [TestCase(false, false)]
    [TestCase(true, true)]
    public void HasLastDateStarts_ReturnsExpectedResult(
        bool hasLastDateStarts,
        bool expected)
    {
        var date = hasLastDateStarts ? DateTime.UtcNow.Date : (DateTime?)null;

        CourseViewModelHelper.HasLastDateStarts(date).Should().Be(expected);
    }

    [TestCase(false, 0, false)]
    [TestCase(true, -1, false)]
    [TestCase(true, 0, true)]
    [TestCase(true, 1, true)]
    public void IsFundingClosing_ReturnsExpectedResult(
        bool hasLastDateStarts,
        int dayOffset,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var date = hasLastDateStarts ? today.AddDays(dayOffset) : (DateTime?)null;

        CourseViewModelHelper.IsFundingClosing(date).Should().Be(expected);
    }

    [TestCase(false, 0, false)]
    [TestCase(true, -1, true)]
    [TestCase(true, 0, false)]
    [TestCase(true, 1, false)]
    public void IsFundingClosed_ReturnsExpectedResult(
        bool hasLastDateStarts,
        int dayOffset,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var date = hasLastDateStarts ? today.AddDays(dayOffset) : (DateTime?)null;

        CourseViewModelHelper.IsFundingClosed(date).Should().Be(expected);
    }

    [Test]
    public void GetFundingStatusText_WhenFundingIsClosing_ReturnsFundingClosingDescription()
    {
        CourseViewModelHelper.GetFundingStatusText(true)
            .Should().Be(FundingStatus.FundingClosing.GetDescription());
    }

    [Test]
    public void GetFundingStatusText_WhenFundingIsNotClosing_ReturnsEmptyString()
    {
        CourseViewModelHelper.GetFundingStatusText(false)
            .Should().BeEmpty();
    }

    [TestCase(true, false, "govuk-tag govuk-tag--orange")]
    [TestCase(true, true, "govuk-tag govuk-tag--orange")]
    [TestCase(false, true, "govuk-tag govuk-tag--grey")]
    [TestCase(false, false, "")]
    public void GetTagColour_ReturnsExpectedColour(
        bool fundingClosing,
        bool fundingClosed,
        string expected)
    {
        CourseViewModelHelper.GetTagColour(fundingClosing, fundingClosed)
            .Should().Be(expected);
    }
}
