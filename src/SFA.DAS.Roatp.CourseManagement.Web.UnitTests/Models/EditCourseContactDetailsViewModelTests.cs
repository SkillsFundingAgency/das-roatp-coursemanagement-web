using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models
{
    [TestFixture]
    public class EditCourseContactDetailsViewModelTests
    {
        [Test, AutoData]
        public void ImplicitOperatorForApiModel_ReturnsViewModel(GetStandardDetailsQueryResult standard)
        {
            var vm = (EditCourseContactDetailsViewModel)standard;

            vm.Should().BeEquivalentTo(standard, o =>
            {
                o.Including(c => c.ContactUsEmail);
                o.Including(c => c.ContactUsPhoneNumber);
                o.Including(c => c.StandardInfoUrl);
                return o;
            });
        }
    }
}
