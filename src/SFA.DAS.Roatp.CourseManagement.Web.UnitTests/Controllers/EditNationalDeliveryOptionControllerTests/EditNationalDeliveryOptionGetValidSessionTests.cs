using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditNationalDeliveryOptionControllerTests
{
    [TestFixture]
    public class EditNationalDeliveryOptionGetValidSessionTests : EditNationalDeliveryOptionControllerTestBase
    {
        protected ViewResult ViewResult;
        protected EditNationalDeliveryOptionViewModel Model;

        [SetUp]
        public void Before_Each_Test()
        {
            SetupController();
            SetLocationOptionInSession(LocationOption.Both);
            ViewResult = (ViewResult)Sut.Index(LarsCode);
            Model = (EditNationalDeliveryOptionViewModel)ViewResult.Model;
        }

        [Test]
        public void ThenReturnsViewResult()
        {
            ViewResult.Should().NotBeNull();
        }

        [Test]
        public void ThenSetViewModelOnTheResult()
        {
            Model.Should().NotBeNull();
        }
    }
}
