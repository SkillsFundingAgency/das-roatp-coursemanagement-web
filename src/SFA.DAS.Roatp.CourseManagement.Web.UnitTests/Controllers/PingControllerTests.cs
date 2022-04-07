using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers
{
    [TestFixture]
    public class PingControllerTests
    {
        [Test]
        public void Ping_returns_Pong()
        {
            var controller = new PingController();
            
            var expectedResponse = "Pong";

            var result = controller.Index();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
            Assert.That((result as OkObjectResult).Value, Is.EqualTo(expectedResponse));
        }
    }
}
