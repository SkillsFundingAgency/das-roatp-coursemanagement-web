using AutoFixture.NUnit4;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.ManageShortCourses;

public class DeleteShortCourseViewModelTest
{
    [Test, AutoData]
    public void DeleteShortCourseViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        GetProviderCourseDetailsQueryResult source)
    {
        // Act
        DeleteShortCourseViewModel sut = source;

        // Assert
        sut.ShortCourseInformation.Should().BeEquivalentTo(source, option =>
        {
            option.WithMapping<ShortCourseInformationViewModel>(r => r.CourseName, m => m.CourseName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.RegulatorName, v => v.RegulatorName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Sector, v => v.Sector);
            option.ExcludingMissingMembers();
            return option;
        });
    }
}
