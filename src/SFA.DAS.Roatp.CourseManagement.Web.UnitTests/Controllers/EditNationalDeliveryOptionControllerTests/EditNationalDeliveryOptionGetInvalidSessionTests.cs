using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests
{
    [TestFixture]
    public class EditNationalDeliveryOptionGetInvalidSessionTests : EditNationalDeliveryOptionControllerTestBase
    {
        [TestCase(LocationOption.ProviderLocation)]
        [TestCase(LocationOption.None)]
        public async Task Get_SessionIsInvalid_RedirectToLocationOptionQuestion(LocationOption option)
        {
            SetupController();

            SetLocationOptionInSession(option);

            SetUpCorrectCourseTypeGetProviderCourseDetailsApiResponse();

            var result = await Sut.Index(LarsCode) as RedirectToRouteResult;

            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetLocationOption);
        }

        [Test]
        public async Task Get_GetStandardsDetailsApiReturnsIncorrectCourseType_RedirectsPageNotFounds()
        {
            SetupController();

            SetUpIncorrectCourseTypeGetProviderCourseDetailsApiResponse();

            var result = await Sut.Index(LarsCode);

            var viewResult = result as ViewResult;
            viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        }
    }
}
