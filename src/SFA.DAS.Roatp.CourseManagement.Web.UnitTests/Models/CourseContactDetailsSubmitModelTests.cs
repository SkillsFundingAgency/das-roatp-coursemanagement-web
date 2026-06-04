using AutoFixture.NUnit4;
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

            command.Should().BeEquivalentTo(sut, options => options
            .Excluding(x => x.LearningType)
            .Excluding(x => x.LearningTypeLower)
            .Excluding(x => x.LearningTypeLowerPlural)
            .Excluding(x => x.LearningTypeHumanize));
        }
    }
}
