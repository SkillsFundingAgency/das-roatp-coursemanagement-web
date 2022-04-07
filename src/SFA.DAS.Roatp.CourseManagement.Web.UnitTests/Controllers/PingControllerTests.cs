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

            var result = controller.Index() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }
    }
}
