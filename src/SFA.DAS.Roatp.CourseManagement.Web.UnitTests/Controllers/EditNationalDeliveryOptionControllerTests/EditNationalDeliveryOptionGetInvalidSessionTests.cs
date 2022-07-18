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
        public void Get_SessionIsInvalid_RedirectToLocationOptionQuestion(LocationOption option)
        {
            SetupController();

            SetLocationOptionInSession(option);

            var result = (RedirectToRouteResult)Sut.Index(LarsCode);

            result.Should().NotBeNull();
            result.RouteName.Should().Be(RouteNames.GetLocationOption);
        }
    }
}
