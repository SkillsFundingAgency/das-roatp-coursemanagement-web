using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.Standards
{
    class ConfirmRegulatedStandardViewModelTests
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
    }
}
