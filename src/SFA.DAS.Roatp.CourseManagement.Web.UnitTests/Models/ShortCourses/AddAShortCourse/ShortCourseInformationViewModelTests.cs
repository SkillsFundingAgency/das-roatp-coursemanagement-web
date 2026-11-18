using System;
using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;

public class ShortCourseInformationViewModelTests
{
    [Test, AutoData]
    public void Operator_TransformsFromGetStandardInformationQueryResult(GetStandardInformationQueryResult source)
    {
        ShortCourseInformationViewModel sut = source;

        sut.Should().BeEquivalentTo(source, option =>
        {
            option.WithMapping<ShortCourseInformationViewModel>(r => r.Title, m => m.CourseName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Route, v => v.Sector);
            option.ExcludingMissingMembers();
            return option;
        });
    }

    [Test, AutoData]
    public void Operator_CourseDisplayName_ReturnsFormattedValue(GetStandardInformationQueryResult source)
    {
        ShortCourseInformationViewModel sut = source;

        var result = sut.CourseDisplayName;

        result.Should().Be($"{sut.CourseName} (level {sut.Level})");
    }

    [Test, AutoData]
    public void Operator_TransformsFromGetProviderAvailableCourseDetailsQueryResult(GetProviderAvailableCourseDetailsQueryResult source)
    {
        ShortCourseInformationViewModel sut = source;

        sut.Should().BeEquivalentTo(source, option =>
        {
            option.WithMapping<ShortCourseInformationViewModel>(c => c.LarsCode, v => v.LarsCode);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Level, v => v.Level);
            option.WithMapping<ShortCourseInformationViewModel>(r => r.Title, m => m.CourseName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Sector, v => v.Sector);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.LastDateStarts, v => v.LastDateStarts);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.IfateReferenceNumber, v => v.IfateReferenceNumber);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.LearningType, v => v.LearningType);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.LearningType, v => v.LearningType);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Duration, v => v.Duration);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.DurationUnits, v => v.DurationUnits);
            return option;
        });
    }

    [TestCase(false, false)]
    [TestCase(true, true)]
    public void Operator_SetsHasLastDateStarts(
        bool hasLastDateStarts,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var source = new GetProviderAvailableCourseDetailsQueryResult()
        {
            LastDateStarts = hasLastDateStarts ? today : null
        };

        ShortCourseInformationViewModel sut = source;

        sut.HasLastDateStarts.Should().Be(expected);
    }

    [TestCase(false, 0, false)]
    [TestCase(true, -1, false)]
    [TestCase(true, 0, true)]
    [TestCase(true, 1, true)]
    public void Operator_SetsFundingClosing(
        bool hasLastDateStarts,
        int dayOffset,
        bool expected)
    {
        var today = DateTime.UtcNow.Date;
        var source = new GetProviderAvailableCourseDetailsQueryResult
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        ShortCourseInformationViewModel sut = source;

        sut.FundingClosing.Should().Be(expected);
    }

    [TestCase(false, 0, "")]
    [TestCase(true, -1, "")]
    [TestCase(true, 1, "Funding closing for new starts")]
    [TestCase(true, 0, "Funding closing for new starts")]
    public void Operator_SetsStatusText(
        bool hasLastDateStarts,
        int dayOffset,
        string expected)
    {
        var today = DateTime.UtcNow.Date;
        var source = new GetProviderAvailableCourseDetailsQueryResult
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        ShortCourseInformationViewModel sut = source;

        sut.StatusText.Should().Be(expected);
    }

    [TestCase(false, 0, "")]
    [TestCase(true, -1, "")]
    [TestCase(true, 1, "govuk-tag govuk-tag--orange")]
    [TestCase(true, 0, "govuk-tag govuk-tag--orange")]
    public void Operator_SetsTagColour(
        bool hasLastDateStarts,
        int dayOffset,
        string expected)
    {
        var today = DateTime.UtcNow.Date;
        var source = new GetProviderAvailableCourseDetailsQueryResult
        {
            LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
        };

        ShortCourseInformationViewModel sut = source;

        sut.TagColour.Should().Be(expected);
    }
}