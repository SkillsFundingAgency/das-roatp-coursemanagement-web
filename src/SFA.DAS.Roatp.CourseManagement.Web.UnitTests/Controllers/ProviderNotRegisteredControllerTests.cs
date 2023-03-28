using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers;
public class ProviderNotRegisteredControllerTests
{
    [Test]
    [MoqAutoData]
    public void Index_ReturnsViewResult([Greedy] ProviderNotRegisteredController sut)
    {
        var result = sut.Index();

        result.As<ViewResult>().ViewName.Should().Be("~/Views/ShutterPages/ProviderNotRegistered.cshtml");
    }
}
