using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.UpdateContactDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models
{
    [TestFixture]
    public class CourseContactDetailsSubmitModelTests
    {
        [Test, AutoData]
        public void ImplicitOperatorForCommand_ReturnsCommand(CourseContactDetailsSubmitModel sut)
        {
            var command = (UpdateProviderCourseContactDetailsCommand)sut;

            command.Should().BeEquivalentTo(sut);
        }
    }
}
