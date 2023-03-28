using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers;

[TestFixture]
public class ControllerAuthorizeAttributeTests
{

    private readonly List<string> _controllersThatDoNotRequireAuthorize = new List<string>()
    {
        "PingController", "ProviderAccountController", "ErrorController", "ControllerBase", "AddAStandardControllerBase", "ProviderNotRegisteredController"
    };

    [Test]
    public void ControllersShouldHaveAuthorizeAttribute()
    {
        var webAssembly = typeof(ProviderAccountController).GetTypeInfo().Assembly;

        var controllers = webAssembly.DefinedTypes.Where(c => c.IsSubclassOf(typeof(Controller))).ToList();

        foreach (var controller in controllers.Where(c => !_controllersThatDoNotRequireAuthorize.Contains(c.Name)))
        {
            controller.Should().BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}
