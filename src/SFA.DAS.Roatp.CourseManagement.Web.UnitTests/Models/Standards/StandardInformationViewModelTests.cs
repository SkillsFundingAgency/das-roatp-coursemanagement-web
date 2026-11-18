using System;
using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Courses.Queries.GetProviderAvailableCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardInformationViewModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsFromGetStandardInformationQueryResult(GetStandardInformationQueryResult source)
        {
            StandardInformationViewModel sut = source;

            sut.Should().BeEquivalentTo(source, option =>
            {
                option.WithMapping<StandardInformationViewModel>(r => r.Title, m => m.CourseName);
                option.WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
                option.WithMapping<StandardInformationViewModel>(c => c.Route, v => v.Sector);
                option.Excluding(r => r.StandardUId);
                option.Excluding(r => r.IsRegulatedForProvider);
                option.Excluding(c => c.Duration);
                option.Excluding(c => c.DurationUnits);
                option.Excluding(c => c.CourseType);
                return option;
            });
        }

        [Test, AutoData]
        public void Operator_TransformsFromGetProviderAvailableCourseDetailsQueryResult(GetProviderAvailableCourseDetailsQueryResult source)
        {
            StandardInformationViewModel sut = source;

            sut.Should().BeEquivalentTo(source, option =>
            {
                option.WithMapping<StandardInformationViewModel>(c => c.LarsCode, v => v.LarsCode);
                option.WithMapping<StandardInformationViewModel>(c => c.Level, v => v.Level);
                option.WithMapping<StandardInformationViewModel>(r => r.Title, m => m.CourseName);
                option.WithMapping<StandardInformationViewModel>(c => c.Sector, v => v.Sector);
                option.WithMapping<StandardInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
                option.WithMapping<StandardInformationViewModel>(c => c.LastDateStarts, v => v.LastDateStarts);
                option.WithMapping<StandardInformationViewModel>(c => c.IfateReferenceNumber, v => v.IfateReferenceNumber);
                option.WithMapping<StandardInformationViewModel>(c => c.LearningType, v => v.LearningType);
                option.ExcludingMissingMembers();
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

            StandardInformationViewModel sut = source;

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

            StandardInformationViewModel sut = source;

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

            StandardInformationViewModel sut = source;

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

            StandardInformationViewModel sut = source;

            sut.TagColour.Should().Be(expected);
        }
    }
}
