using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddAStandard
{
    [TestFixture]
    public class CourseLocationModelTests
    {
        [Test, AutoData]
        public void OperatorProviderCourseLocationCommandModel_TransformsToCommandModel(CourseLocationModel sut)
        {
            ProviderCourseLocationCommandModel model = sut;

            model.Should().BeEquivalentTo(sut, options => options.ExcludingMissingMembers());
        }

        [Test, AutoData]
        public void OperatorRegionModel_TransformsFromRegionModel(RegionModel model)
        {
            CourseLocationModel sut = model;
            sut.Should().BeEquivalentTo(model, options => options.ExcludingMissingMembers());
            sut.LocationType.Should().Be(LocationType.Regional);
        }
    }
}
