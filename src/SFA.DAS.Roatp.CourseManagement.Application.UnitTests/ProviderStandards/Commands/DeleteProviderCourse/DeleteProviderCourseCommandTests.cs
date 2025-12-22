using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.DeleteProviderCourse
{
    [TestFixture]
    public class DeleteProviderCourseCommandTests
    {
        [Test, AutoData]
        public void Constructor_BuildsCommand(int ukprn, string larsCode, string userId, string userDisplayName)
        {
            var sut = new DeleteProviderCourseCommand(ukprn, larsCode, userId, userDisplayName);
            sut.Ukprn.Should().Be(ukprn);
            sut.LarsCode.Should().Be(larsCode);
            sut.UserId.Should().Be(userId);
            sut.UserDisplayName.Should().Be(userDisplayName);
        }
    }
}
