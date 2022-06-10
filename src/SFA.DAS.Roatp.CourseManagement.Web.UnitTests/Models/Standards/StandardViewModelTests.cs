using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{

    [TestFixture]
    public class StandardViewModelTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ImplicitOperater_ConvertsFromStandard(bool approvedByRegulator)
        {
            const int providerCourseId = 1;
            const string courseName = "course name";
            const int level = 1;
            const int larsCode = 133;
            const string version = "1.1";
            const string approvalBody = "approval body";
            var isApprovedByRegulator = approvedByRegulator;

            var standard = new Standard
            {
                ProviderCourseId = providerCourseId,
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
                Version = version,
                ApprovalBody = approvalBody,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            StandardViewModel viewModel = standard;

            viewModel.ProviderCourseId.Should().Be(providerCourseId);
            viewModel.CourseName.Should().Be(courseName);
            viewModel.Level.Should().Be(level);
            viewModel.Version.Should().Be(version);
            viewModel.ApprovalBody.Should().Be(approvalBody);
            viewModel.LarsCode.Should().Be(larsCode);
            viewModel.IsApprovedByRegulator.Should().Be(isApprovedByRegulator);
            viewModel.ApprovalRequired.Should().Be(!isApprovedByRegulator);
        }
    }
}
