using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.ManageShortCourses;
public class DeleteShortCourseViewModelTest
{
    [Test, AutoData]
    public void DeleteShortCourseViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        GetStandardInformationQueryResult source)
    {
        // Act
        DeleteShortCourseViewModel sut = source;

        // Assert
        sut.ShortCourseInformation.Should().BeEquivalentTo(source, option =>
        {
            option.WithMapping<ShortCourseInformationViewModel>(r => r.Title, m => m.CourseName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Route, v => v.Sector);
            option.Excluding(r => r.StandardUId);
            return option;
        });
    }
}
