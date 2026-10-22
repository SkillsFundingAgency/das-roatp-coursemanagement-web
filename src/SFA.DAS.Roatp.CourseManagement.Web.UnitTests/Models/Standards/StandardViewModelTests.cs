using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardViewModelTests
    {
        [TestCase(true, false, false)]
        [TestCase(null, true, true)]
        [TestCase(null, false, false)]
        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        public void ImplicitOperator_ConvertsFromStandard(bool? approvedByRegulator, bool isRegulatedForProvider, bool expectedApprovalRequired)
        {
            const int providerCourseId = 1;
            const string courseName = "course name";
            const int level = 1;
            const string larsCode = "133";
            const string expectedCourseDisplayName = "course name (level 1)";
            const string approvalBody = "approval body";
            const bool hasLocations = true;
            DateTime? lastDateStarts = DateTime.UtcNow.Date;

            var standard = new Standard
            {
                ProviderCourseId = providerCourseId,
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                ApprovalBody = approvalBody,
                IsApprovedByRegulator = approvedByRegulator,
                IsRegulatedForProvider = isRegulatedForProvider,
                HasLocations = hasLocations,
                LastDateStarts = lastDateStarts
            };

            StandardViewModel viewModel = standard;


            viewModel.ProviderCourseId.Should().Be(providerCourseId);
            viewModel.CourseName.Should().Be(courseName);
            viewModel.Level.Should().Be(level);
            viewModel.ApprovalBody.Should().Be(approvalBody);
            viewModel.LarsCode.Should().Be(larsCode);
            viewModel.IsApprovedByRegulator.Should().Be(approvedByRegulator);
            viewModel.IsApprovalPending.Should().Be(expectedApprovalRequired);
            viewModel.CourseDisplayName.Should().Be(expectedCourseDisplayName);
            viewModel.IsRegulatedForProvider.Should().Be(isRegulatedForProvider);
            viewModel.HasLocation.Should().Be(hasLocations);
            viewModel.LastDateStarts.Should().Be(lastDateStarts);
        }

        [TestCase(false, false)]
        [TestCase(true, true)]
        public void WhenDifferentLastDateStartsValuesArePassed_ThenHasLastDateStartsIsSet(
            bool hasLastDateStarts,
            bool expected)
        {
            var today = DateTime.UtcNow.Date;
            var viewModel = new StandardViewModel
            {
                LastDateStarts = hasLastDateStarts ? today : null
            };

            viewModel.HasLastDateStarts.Should().Be(expected);
        }

        [TestCase(false, 0, false)]
        [TestCase(true, -1, false)]
        [TestCase(true, 0, false)]
        [TestCase(true, 1, true)]
        public void WhenDifferentLastDateStartsValuesArePassed_ThenFundingClosingIsSet(
            bool hasLastDateStarts,
            int dayOffset,
            bool expected)
        {
            var today = DateTime.UtcNow.Date;
            var viewModel = new StandardViewModel
            {
                LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
            };

            viewModel.FundingClosing.Should().Be(expected);
        }

        [TestCase(false, 0, false)]
        [TestCase(true, -1, true)]
        [TestCase(true, 0, true)]
        [TestCase(true, 1, false)]
        public void WhenDifferentLastDateStartsValuesArePassed_ThenFundingClosedIsSet(
            bool hasLastDateStarts,
            int dayOffset,
            bool expected)
        {
            var today = DateTime.UtcNow.Date;
            var viewModel = new StandardViewModel
            {
                LastDateStarts = hasLastDateStarts ? today.AddDays(dayOffset) : null
            };

            viewModel.FundingClosed.Should().Be(expected);
        }
    }
}
