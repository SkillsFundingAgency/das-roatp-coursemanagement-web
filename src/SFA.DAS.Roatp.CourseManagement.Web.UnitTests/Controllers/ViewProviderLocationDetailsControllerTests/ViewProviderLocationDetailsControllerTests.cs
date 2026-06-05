using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit4;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderLocations.Queries.GetProviderLocationDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ViewProviderLocationDetailsControllerTests
{
    [TestFixture]
    public class ViewProviderLocationDetailsControllerTests
    {
        private const string Ukprn = "10012002";
        private Mock<IMediator> _mediatorMock;
        private ViewProviderLocationDetailsController _sut;
        private readonly string verifyUrl = "http://test";
        private readonly string verifyUpdateProviderLocationDetailsUrl = "http://test-UpdateProviderLocationDetailsUrl";
        private readonly string verifySelectCourseTypeUrl = "http://test-SelectCourseType";
        private readonly string standardLinkUrl = "http://test-StandardLink";
        private readonly string apprenticeshipUnitUrl = "http://test-ApprenticeshipUnitLink";

        [SetUp]
        public void Before_Each_Test()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new ViewProviderLocationDetailsController(_mediatorMock.Object, Mock.Of<ILogger<ViewProviderLocationDetailsController>>());
            _sut.AddDefaultContextWithUser()
                .AddUrlHelperMock()
                .AddUrlForRoute(RouteNames.GetProviderLocations, verifyUrl)
                .AddUrlForRoute(RouteNames.GetUpdateProviderLocationDetails, verifyUpdateProviderLocationDetailsUrl)
                .AddUrlForRoute(RouteNames.SelectCourseType, verifySelectCourseTypeUrl)
                .AddUrlForRoute(RouteNames.GetStandardDetails, standardLinkUrl)
                .AddUrlForRoute(RouteNames.ManageShortCourseDetails, apprenticeshipUnitUrl);
        }

        [Test, AutoData]
        public async Task GetProviderLocationDetails_ValidRequest_ReturnsView(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Contain("ViewProviderLocationsDetails.cshtml");
            var model = viewResult.Model as ProviderLocationViewModel;
            model.Should().NotBeNull();
            model.UpdateContactDetailsUrl.Should().Be(verifyUpdateProviderLocationDetailsUrl);
            model.ManageYourStandardsUrl.Should().Be(verifySelectCourseTypeUrl);
        }

        [Test, AutoData]
        public async Task GetProviderLocationDetails_InvalidRequest_ReturnsPageNotFound(Guid id)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetProviderLocationDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetProviderLocationDetailsQueryResult)null);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
        }

        [Test, AutoData]
        public async Task WhenStandardsInlcudeApprenticeshipAndFoundationApprenticeship_ThenPopulateStandardCourseLinks(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test A Standard",
                    Level = 2,
                    LarsCode = "12345678",
                    LearningType = LearningType.Apprenticeship
                },
                new LocationStandardModel()
                {
                    Title = "Test B Foundation Apprenticeship",
                    Level = 2,
                    LarsCode = "23456787",
                    LearningType = LearningType.FoundationApprenticeship
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as ProviderLocationViewModel;
            var standardCourses = model.StandardLinks.Courses.ToList();
            standardCourses[0].CourseName.Should().Be("Test A Standard (level 2)");
            standardCourses[0].Url.Should().Be(standardLinkUrl);
            standardCourses[1].CourseName.Should().Be("Test B Foundation Apprenticeship (level 2)");
            standardCourses[1].Url.Should().Be(standardLinkUrl);
            model.StandardLinks.Courses.Count().Should().Be(2);
        }

        [Test, AutoData]
        public async Task WhenStandardsApprenticeshipUnit_ThenPopulateApprenticeshipUnitCourseLinks(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test Apprenticeship Unit",
                    Level = 2,
                    LarsCode = "98765432",
                    LearningType = LearningType.ApprenticeshipUnit
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as ProviderLocationViewModel;
            model.ApprenticeshipUnitLinks.Courses.First().CourseName.Should().Be("Test Apprenticeship Unit (level 2)");
            model.ApprenticeshipUnitLinks.Courses.First().Url.Should().Be(apprenticeshipUnitUrl);
        }

        [Test, AutoData]
        public async Task WhenStandardsInlcudeApprenticeshipAndApprenticeshipUnit_ThenCourseFlagsAreSetToTrue(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            queryResult.ProviderLocation.Standards = new List<LocationStandardModel>()
            {
                new LocationStandardModel()
                {
                    Title = "Test Standard",
                    Level = 2,
                    LarsCode = "12345678",
                    LearningType = LearningType.Apprenticeship
                },
                new LocationStandardModel()
                {
                    Title = "Test Foundation Apprenticeship",
                    Level = 2,
                    LarsCode = "23456787",
                    LearningType = LearningType.FoundationApprenticeship
                },
                new LocationStandardModel()
                {
                    Title = "Test Apprenticeship Unit",
                    Level = 2,
                    LarsCode = "98765432",
                    LearningType = LearningType.ApprenticeshipUnit
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as ProviderLocationViewModel;
            model.HasCourses.Should().BeTrue();
            model.ShowStandards.Should().BeTrue();
            model.ShowApprenticeshipUnits.Should().BeTrue();
        }

        [Test, AutoData]
        public async Task WhenStandardsIsEmpty_ThenCourseFlagsAreSetToFalse(
            GetProviderLocationDetailsQueryResult queryResult,
            Guid id)
        {
            queryResult.ProviderLocation.Standards = new List<LocationStandardModel>();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetProviderLocationDetailsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _sut.GetProviderLocationDetails(id);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as ProviderLocationViewModel;
            model.HasCourses.Should().BeFalse();
            model.ShowStandards.Should().BeFalse();
            model.ShowApprenticeshipUnits.Should().BeFalse();
        }
    }
}
