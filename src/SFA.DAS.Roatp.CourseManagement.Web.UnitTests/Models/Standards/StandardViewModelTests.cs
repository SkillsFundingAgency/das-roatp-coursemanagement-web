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
            var standard = new Standard();

            StandardViewModel viewModel = standard;

            viewModel.Should().BeEquivalentTo(viewModel);
        }
    }
}
