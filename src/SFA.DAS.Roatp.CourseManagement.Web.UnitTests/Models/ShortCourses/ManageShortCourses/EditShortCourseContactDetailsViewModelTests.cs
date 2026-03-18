using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.ManageShortCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.ShortCourses.ManageShortCourses;
public class EditShortCourseContactDetailsViewModelTests
{
    [Test, MoqAutoData]
    public void EditShortCourseContactDetailsViewModel_ImplicitOperator_MapsPropertiesCorrectly(
        GetStandardDetailsQueryResult source)
    {
        // Act
        EditShortCourseContactDetailsViewModel sut = source;

        // Assert
        Assert.That(sut.ContactUsEmail, Is.EqualTo(source.ContactUsEmail));
        Assert.That(sut.StandardInfoUrl, Is.EqualTo(source.StandardInfoUrl));
        Assert.That(sut.ContactUsPhoneNumber, Is.EqualTo(source.ContactUsPhoneNumber));
    }
}
