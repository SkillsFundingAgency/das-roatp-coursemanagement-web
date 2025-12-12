using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.StandardsControllerTests
{
    [TestFixture]
    public class ViewStandardTests
    {
        private StandardsController _controller;
        private Mock<ILogger<StandardsController>> _logger;
        private Mock<IMediator> _mediator;
        private const string LarsCode = "123";
        private const ApprenticeshipType ApprenticeshipType = Domain.ApiModels.ApprenticeshipType.Apprenticeship;
        private const string verifyUrl = "http://test";
        private const string verifyEditContactDetailsUrl = "http://test-verifyEditContactDetailsUrl";
        private const string verifyEditLocationOptionUrl = "http://test-verifyEditLocationOptionUrl";
        private const string Regulator = "Test-Regulator";
        private const string verifyeditProviderCourseRegionsUrl = "http://update-standardSubRegions";
        private const string verifyEditTrainingLocationsUrl = "http://test-verifyEditTrainingLocationsUrl";
        private const string verifyConfirmRegulatedStandardUrl = "http://test-verifyConfirmRegulatedStandardUrl";
        private const string verifydeleteStandardUrl = "http://test-delete-standard-url";


        [SetUp]
        public void Before_each_test()
        {
            _logger = new Mock<ILogger<StandardsController>>();

            var response = new GetStandardDetailsQueryResult
            {
                CourseName = "test1",
                Level = 1,
                IFateReferenceNumber = "1234",
                Sector = "Digital",
                LarsCode = LarsCode,
                RegulatorName = "",
                ApprenticeshipType = ApprenticeshipType,
                StandardInfoUrl = "www.test.com",
                ContactUsEmail = "test@test.com",
                ContactUsPhoneNumber = "123456789",
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true,
                ProviderCourseLocations = new System.Collections.Generic.List<ProviderCourseLocation> { new ProviderCourseLocation { LocationType = LocationType.Regional, RegionName = "Region1" } }
            };

            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.ViewStandards, verifyUrl);
        }

        [Test]
        public async Task ViewStandard_ReturnsValidResponse()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true,
                LarsCode = LarsCode
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("ViewStandardDetails.cshtml");
            viewResult.Model.Should().NotBeNull();
            var model = viewResult.Model as StandardDetailsViewModel;
            model.Should().NotBeNull();
            model!.StandardInformation.LarsCode.Should().Be(LarsCode);
            model.StandardInformation.ApprenticeshipType.Should().Be(ApprenticeshipType);
            model.BackUrl.Should().Be(verifyUrl);
            _logger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }

        [Test]
        public async Task ViewStandard_HandlerReturnsNull_ThrowsInvalidOperaionException()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => null);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock();

            Func<Task> action = () => _controller.ViewStandard(LarsCode);

            await action.Should().ThrowAsync<InvalidOperationException>();
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task ViewStandard_ResponseIncludesRegulator()
        {
            var response = new GetStandardDetailsQueryResult
            {
                RegulatorName = Regulator,
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock();

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as StandardDetailsViewModel;
            model.Should().NotBeNull();
            model.StandardInformation.RegulatorName.Should().Be(Regulator);
            model.IsRegulatedForProvider.Should().Be(true);
        }

        [Test]
        public async Task ViewStandard_ResponseIndicatesHasLocations()
        {
            var response = new GetStandardDetailsQueryResult
            {
                HasLocations = true,
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock();

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            var model = viewResult.Model as StandardDetailsViewModel;
            model.Should().NotBeNull();
            model.HasLocations.Should().Be(true);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditContactDetailsUrl()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetCourseContactDetails, verifyEditContactDetailsUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditContactDetailsUrl.Should().Be(verifyEditContactDetailsUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditLocationOptionUrl()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetLocationOption, verifyEditLocationOptionUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditLocationOptionUrl.Should().Be(verifyEditLocationOptionUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditProviderCourseRegionsUrl()
        {
            var response = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = new System.Collections.Generic.List<ProviderCourseLocation> { new ProviderCourseLocation { LocationType = LocationType.Regional, RegionName = "Region1" } },
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetStandardSubRegions, verifyeditProviderCourseRegionsUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditProviderCourseRegionsUrl.Should().Be(verifyeditProviderCourseRegionsUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesEditProviderCourseRegionsUrlEmpty()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetStandardSubRegions, verifyeditProviderCourseRegionsUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditProviderCourseRegionsUrl.Should().BeEmpty();
        }

        [Test]
        public async Task ViewStandard_PopulatesEditTrainingLocationsUrl()
        {
            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderCourseLocations, verifyEditTrainingLocationsUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.EditTrainingLocationsUrl.Should().Be(verifyEditTrainingLocationsUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesConfirmRegulatedStandardUrl()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetConfirmRegulatedStandard, verifyConfirmRegulatedStandardUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.ConfirmRegulatedStandardUrl.Should().Be(verifyConfirmRegulatedStandardUrl);
        }

        [Test]
        public async Task ViewStandard_PopulatesDeleteStandardUrl()
        {
            var response = new GetStandardDetailsQueryResult
            {
                IsRegulatedForProvider = true,
                IsApprovedByRegulator = true
            };

            _mediator.Setup(x => x.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _controller = new StandardsController(_mediator.Object, _logger.Object);
            _controller
                .AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetConfirmDeleteStandard, verifydeleteStandardUrl);

            var result = await _controller.ViewStandard(LarsCode);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as StandardDetailsViewModel;
            model.DeleteStandardUrl.Should().Be(verifydeleteStandardUrl);
        }
    }
}
