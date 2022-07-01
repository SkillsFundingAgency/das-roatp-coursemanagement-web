using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Regions.Commands.UpdateStandardSubRegions;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class RegionsViewModelTests
    {
        [Test, AutoData]
        public void ImplicitOperatorForCommand_ReturnsCommand(RegionsViewModel model)
        {
            var command = (UpdateStandardSubRegionsCommand)model;

            command.Should().BeEquivalentTo(model, o =>
            {
                o.Including(c => c.LarsCode);
                return o;
            });
        }
    }
}
