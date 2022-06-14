using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Commands.UpdateApprovedByRegulator;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    [TestFixture]
    public class ConfirmRegulatedStandardViewModelTests
    {
        [Test, AutoData]
        public void ImplicitOperatorForApiModel_ReturnsViewModel(StandardDetails standard)
        {
            var vm = (ConfirmRegulatedStandardViewModel)standard;

            vm.Should().BeEquivalentTo(standard, o =>
            {
                o.Including(c => c.RegulatorName);
                o.Including(c => c.IsApprovedByRegulator);
                return o;
            });
        }

        [Test, AutoData]
        public void ImplicitOperatorForCommand_ReturnsCommand(ConfirmRegulatedStandardViewModel model)
        {
            var vm = (UpdateApprovedByRegulatorCommand)model;

            vm.Should().BeEquivalentTo(model, o =>
            {
                o.Including(c => c.LarsCode);
                o.Including(c => c.IsApprovedByRegulator);
                return o;
            });
        }
    }
}
