using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{

    [TestFixture]
    public class StandardViewModelTests
    {
        [Test]
        public void ImplicitOperater_ConvertsFromStandard()
        {
            const int providerCourseId = 1;
            const string courseName = "course name";
            const int level = 1;
            const int larsCode = 133;

            var standard = new Standard
            {
                ProviderCourseId = providerCourseId,
                CourseName = courseName,
                LarsCode = larsCode,
                Level = level,
            };

            StandardViewModel viewModel = standard;

            viewModel.ProviderCourseId.Should().Be(providerCourseId);
            viewModel.CourseName.Should().Be(courseName);
            viewModel.Level.Should().Be(level);
            viewModel.LarsCode.Should().Be(larsCode);
            viewModel.Should().BeEquivalentTo(viewModel);

        }
    }
}
