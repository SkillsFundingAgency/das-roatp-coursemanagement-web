using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class ReviewYourDetailsControllerTests
    {
        [Test]
        public void Index_ReturnsViewWithModel()
        {
            var mockOptions = new Mock<IOptions<ProviderSharedUIConfiguration>>();
            ProviderSharedUIConfiguration config = new ProviderSharedUIConfiguration() { DashboardUrl = @"https://dashboard.com" };
            var expectedModel = new ReviewYourDetailsViewModel() { DashboardUrl = config.DashboardUrl };
            mockOptions.Setup(o => o.Value).Returns(config);
            var sut = new ReviewYourDetailsController(mockOptions.Object);

            var result = sut.ReviewYourDetails() as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().Contain(nameof(ReviewYourDetailsController.ReviewYourDetails));
            result.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
