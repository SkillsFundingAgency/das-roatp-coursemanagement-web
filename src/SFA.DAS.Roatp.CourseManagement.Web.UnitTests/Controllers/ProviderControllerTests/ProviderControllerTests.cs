using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Providers.Queries;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderDescription;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderControllerTests
{
    [TestFixture]
    public class ProviderControllerTests
    {
        private ProviderController _controller;
        private Mock<ILogger<ProviderController>> _logger;
        private Mock<IMediator> _mediator;

        public const int Ukprn = 12345678;
        public const string MarketingInfo = "Marketing info";

        private static string ReviewYourDetailsLink = Guid.NewGuid().ToString();

        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<ProviderController>>();
            var provider = new Domain.ApiModels.Provider
            {
                MarketingInfo = MarketingInfo
            };


            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetProviderQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new GetProviderQueryResult
                {
                    Provider = provider
                });

            _controller = new ProviderController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.ReviewYourDetails, ReviewYourDetailsLink);
        }

        [Test]
        public async Task ProviderController_ViewProviderDescription_ReturnsValidResponse()
        {
            var result = await _controller.ViewProviderDescription(Ukprn);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ProviderDescription/Index.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as ProviderDescriptionViewModel;
            model.Should().NotBeNull();
            model.Description = MarketingInfo;
            model.BackUrl.Should().Be(ReviewYourDetailsLink);
        }
    }
}
