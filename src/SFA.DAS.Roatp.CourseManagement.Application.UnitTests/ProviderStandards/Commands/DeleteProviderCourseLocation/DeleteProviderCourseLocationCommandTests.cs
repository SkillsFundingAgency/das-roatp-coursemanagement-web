using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using System;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.DeleteProviderCourseLocation
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandTests
    {
        [Test, AutoData]
        public void Constructor_BuildsCommand(int ukprn, string larsCode, Guid id, string userId, string userDisplayName)
        {
            var sut = new DeleteProviderCourseLocationCommand(ukprn, larsCode, id, userId, userDisplayName);
            sut.Ukprn.Should().Be(ukprn);
            sut.LarsCode.Should().Be(larsCode);
            sut.Id.Should().Be(id);
            sut.UserId.Should().Be(userId);
            sut.UserDisplayName.Should().Be(userDisplayName);
        }
    }
}
