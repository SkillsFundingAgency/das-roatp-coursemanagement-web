using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddTrainingLocation
{
    [TestFixture]
    public class CourseLocationModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsToCommandModel(CourseLocationModel sut)
        {
            ProviderCourseLocationCommandModel model = sut;

            model.Should().BeEquivalentTo(sut, options => options.ExcludingMissingMembers());
        }
    }
}
