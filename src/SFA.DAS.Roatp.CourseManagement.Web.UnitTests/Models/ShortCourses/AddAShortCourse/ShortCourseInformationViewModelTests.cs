using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standards.Queries.GetStandardInformation;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class ShortCourseInformationViewModelTests
{
    [Test, AutoData]
    public void Operator_TransformsFromGetStandardInformationQueryResult(GetStandardInformationQueryResult source)
    {
        ShortCourseInformationViewModel sut = source;

        sut.Should().BeEquivalentTo(source, option =>
        {
            option.WithMapping<ShortCourseInformationViewModel>(r => r.Title, m => m.CourseName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.ApprovalBody, v => v.RegulatorName);
            option.WithMapping<ShortCourseInformationViewModel>(c => c.Route, v => v.Sector);
            option.Excluding(r => r.StandardUId);
            return option;
        });
    }
}