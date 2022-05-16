using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class StandardDetailsViewModelTests
    {
        [Test]
        public void ImplicitOperater_ConvertsFromStandardDetails()
        {
            var standardDetails = new StandardDetails();

            StandardDetailsViewModel viewModel = standardDetails;

            viewModel.Should().BeEquivalentTo(standardDetails);
        }
    }
}
