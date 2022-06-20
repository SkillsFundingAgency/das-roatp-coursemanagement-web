using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models
{
    [TestFixture]
    public class EditCourseContactDetailsViewModelTests
    {
        [Test, AutoData]
        public void ImplicitOperatorForCommand_ReturnsCommand(EditCourseContactDetailsViewModel sut)
        { 
            var command = (UpdateProviderCourseContactDetailsCommand)sut;

            command.Should().BeEquivalentTo(sut, o => 
            {
                o.Excluding(c => c.BackLink);
                o.Excluding(c => c.CancelLink);
                o.Excluding(c => c.ProviderCourseId);
                return o;
            });
        }

        [Test, AutoData]
        public void ImplicitOperatorForApiModel_ReturnsViewModel(GetStandardDetailsQueryResult standard)
        {
            var vm = (EditCourseContactDetailsViewModel)standard;

            vm.Should().BeEquivalentTo(standard, o => 
            {
                o.Including(c => c.ContactUsEmail);
                o.Including(c => c.ContactUsPageUrl);
                o.Including(c => c.ContactUsPhoneNumber);
                o.Including(c => c.StandardInfoUrl);
                return o;
            });
        }
    }
}
