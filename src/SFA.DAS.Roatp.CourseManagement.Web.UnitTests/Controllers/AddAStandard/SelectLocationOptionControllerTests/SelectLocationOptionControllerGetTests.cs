using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.SelectLocationOptionControllerTests
{
    [TestFixture]
    public class SelectLocationOptionControllerGetTests
    {
        [Test, MoqAutoData]
        public void SelectLocationOption_ReturnsView(
            [Greedy] SelectLocationOptionController sut,
            string cancelLink)
        {
            sut.AddDefaultContextWithUser().AddUrlHelperMock().AddUrlForRoute(RouteNames.ViewStandards, cancelLink);

            var result = sut.SelectLocationOption();

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().ViewName.Should().Be(SelectLocationOptionController.ViewPath);
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().Should().NotBeNull();
            result.As<ViewResult>().Model.As<SelectLocationOptionViewModel>().CancelLink.Should().Be(cancelLink);
        }
    }
}
