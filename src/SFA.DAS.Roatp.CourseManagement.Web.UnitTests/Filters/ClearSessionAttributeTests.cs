using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Filters
{
    [TestFixture]
    public class ClearSessionAttributeTests
    {
        [Test]
        public void ClearSession_RemovesSpecificAttribute()
        {
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();
            httpContext.Session = sessionMock.Object;
            var key = "datakey";
            var context = new ActionExecutingContext(
                new ActionContext(
                    httpContext: httpContext,
                    routeData: new RouteData(),
                    actionDescriptor: new ActionDescriptor()
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>());

            var sut = new ClearSessionAttribute(key);

            sut.OnActionExecuting(context);

            sessionMock.Verify(s => s.Remove(key));
        }
    }
}
