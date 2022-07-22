using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Models.AddTrainingLocation
{
    [TestFixture]
    public class TrainingLocationDetailsViewModelTests
    {
        [Test, AutoData]
        public void Constructor_BuildsModel(AddressItem addressItem)
        {
            var sut = new TrainingLocationDetailsViewModel(addressItem);
            sut.AddressLine1.Should().Be(addressItem.AddressLine1);
            sut.AddressLine2.Should().Be(addressItem.AddressLine2);
            sut.Town.Should().Be(addressItem.Town);
            sut.Postcode.Should().Be(addressItem.Postcode);
        }
    }
}
