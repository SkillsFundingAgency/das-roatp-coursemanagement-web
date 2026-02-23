using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Commands.AddProviderCourse;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using System.Linq;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.AddAShortCourse;
public class ShortCourseSessionModelTests
{
    [Test]
    [InlineAutoData(false)]
    [InlineAutoData(true)]
    public void Operator_TransformsToAddProviderCourseCommand(
        bool dataEmpty,
        ShortCourseSessionModel source)
    {
        // Arrange
        if (dataEmpty) { source = new ShortCourseSessionModel(); }

        AddProviderCourseCommand model = source;

        // Asset
        model.LarsCode.Should().BeEquivalentTo(source.LarsCode);
        model.IsApprovedByRegulator.Should().Be(source.ShortCourseInformation.IsRegulatedForProvider ? true : null);
        model.StandardInfoUrl.Should().BeEquivalentTo(source.ContactInformation.StandardInfoUrl);
        model.ContactUsEmail.Should().BeEquivalentTo(source.ContactInformation.ContactUsEmail);
        model.ContactUsPhoneNumber.Should().BeEquivalentTo(source.ContactInformation.ContactUsPhoneNumber);
        model.HasNationalDeliveryOption.Should().Be(source.HasNationalDeliveryOption.GetValueOrDefault());
        model.HasOnlineDeliveryOption.Should().Be(source.HasOnlineDeliveryOption);
        model.SubregionIds.Should().BeEquivalentTo(source.TrainingRegions.Select(l => l.SubregionId.Value).ToList());
        model.ProviderLocations.Should().BeEquivalentTo(source.TrainingVenues.Select(l => (ProviderCourseLocationCommandModel)l).ToList());
    }
}
