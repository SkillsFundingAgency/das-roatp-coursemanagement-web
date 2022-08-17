﻿using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetAllProviderStandards;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Filters;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.StandardsControllerTests
{
    [TestFixture]
    public class ViewStandardsTests
    {
        private StandardsController _controller;
        private Mock<ILogger<StandardsController>> _logger;
        private Mock<IMediator> _mediator;
        private static string ReviewYourDetailsLink = Guid.NewGuid().ToString();
        private static string GetStandardDetailsLink = Guid.NewGuid().ToString();
        private static string GetConfirmRegulatedStandardLink = Guid.NewGuid().ToString();
        private static string AddAStandardLink = Guid.NewGuid().ToString();
        private static string deleteStandardReferer = "delete-standard-Referer";

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<StandardsController>>();

            var standard1 = new Standard
            {
                ProviderCourseId = 1,
                CourseName = "test1",
                Level = 1,
                IsImported = true,
                ApprovalBody = "TestBody1",
            };
            var standard2 = new Standard
            {
                ProviderCourseId = 2,
                CourseName = "test2",
                Level = 2,
                IsImported = false,
                ApprovalBody = null
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetAllProviderStandardsQueryResult
                {
                    Standards = new List<Standard> { standard1, standard2 }
                });

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetStandardDetails, GetStandardDetailsLink)
                .AddUrlForRoute(RouteNames.GetConfirmRegulatedStandard, GetConfirmRegulatedStandardLink)
                .AddUrlForRoute(RouteNames.ReviewYourDetails, ReviewYourDetailsLink)
                .AddUrlForRoute(RouteNames.GetAddStandardSelectStandard, AddAStandardLink);
        }

        [Test]
        public void ViewStandards_ClearsStandardSessionModel()
        {
            var method = typeof(StandardsController).GetMethod(nameof(StandardsController.ViewStandards));
            method.Should().BeDecoratedWith<ClearSessionAttribute>();
            var clearSessionAttribute = method.GetCustomAttributes(false).FirstOrDefault(att => att.GetType().Name == typeof(ClearSessionAttribute).Name);
            clearSessionAttribute.As<ClearSessionAttribute>().SessionKey.Should().Be(nameof(StandardSessionModel));
        }

        [Test]
        public async Task StandardsController_ViewStandards_ReturnsValidResponse()
        {
            _controller.HttpContext.Request.Headers.Add("Referer", deleteStandardReferer);

            var result = await _controller.ViewStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardListViewModel;
            model.Should().NotBeNull();
            model.Standards.Should().NotBeNull();
            model.BackLink.Should().Be(ReviewYourDetailsLink);
            model.Standards.First().StandardUrl.Should().Be(GetStandardDetailsLink);
            model.Standards.First().IsRegulatedStandard.Should().BeTrue();
            model.Standards.First().ConfirmRegulatedStandardUrl.Should().Be(GetConfirmRegulatedStandardLink);
            model.Standards.Last().ConfirmRegulatedStandardUrl.Should().Be(String.Empty);
        }

        [Test]
        public async Task StandardsController_ViewStandards_ReturnsNoStandardData()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetAllProviderStandardsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);
            
            var result = await _controller.ViewStandards();

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandards.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardListViewModel;
            model.Should().NotBeNull();
            model.Standards.Should().BeEmpty();
            model.BackLink.Should().Be(ReviewYourDetailsLink);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }
    }
}
