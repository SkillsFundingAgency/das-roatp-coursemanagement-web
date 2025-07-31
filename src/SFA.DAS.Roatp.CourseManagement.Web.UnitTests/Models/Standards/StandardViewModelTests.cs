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
        [TestCase(null, true, false)]
        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        public void ImplicitOperator_ConvertsFromStandard(bool? approvedByRegulator, bool isRegulatedForProvider, bool expectedApprovalRequired)
        {
            const int providerCourseId = 1;
            const string courseName = "course name";
            const int level = 1;
            const int larsCode = 133;
            const string expectedCourseDisplayName = "course name (level 1)";
            const string approvalBody = "approval body";
            const bool hasLocations = true;

            var standard = new Standard
            {
                ProviderCourseId = providerCourseId,
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                ApprovalBody = approvalBody,
                IsApprovedByRegulator = approvedByRegulator,
                IsRegulatedForProvider = isRegulatedForProvider,
                HasLocations = hasLocations
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
        }
    }
}
