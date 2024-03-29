﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Commands.DeleteCourseLocations;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Commands.DeleteCourseLocations
{
    [TestFixture]
    public class DeleteCourseLocationsCommandTests
    {
        [Test, AutoData]
        public void Constructor_BuildsCommand(int ukprn, int larsCode, string userId, string userDisplayName, DeleteProviderCourseLocationOption option)
        {
            var sut = new DeleteCourseLocationsCommand(ukprn, larsCode, userId, userDisplayName, option);
            sut.Ukprn.Should().Be(ukprn);
            sut.LarsCode.Should().Be(larsCode);
            sut.UserId.Should().Be(userId);
            sut.UserDisplayName.Should().Be(userDisplayName);
            sut.DeleteProviderCourseLocationOption.Should().Be(option);
        }
    }
}
