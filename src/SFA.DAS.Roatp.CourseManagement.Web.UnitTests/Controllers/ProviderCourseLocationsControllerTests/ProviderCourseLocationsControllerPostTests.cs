using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Authorization;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ProviderCourseLocationsControllerTests
{
    [TestFixture]
    public class ProviderCourseLocationsControllerPostTests
    {
        private const string Ukprn = "10012002";
        private static string UserId = Guid.NewGuid().ToString();
        private Mock<ILogger<ProviderCourseLocationsController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private ProviderCourseLocationsController _sut;
        private Mock<IUrlHelper> urlHelper;
        string verifyUrl = "http://test";
        protected Mock<ISessionService> _sessionServiceMock;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<ProviderCourseLocationsController>>();
            _mediatorMock = new Mock<IMediator>();
            _sessionServiceMock = new Mock<ISessionService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new Claim[] { new Claim(ProviderClaims.ProviderUkprn, Ukprn), new Claim(ProviderClaims.UserId, UserId)}, 
                "mock"));
            var httpContext = new DefaultHttpContext() { User = user };

            _sut = new ProviderCourseLocationsController(_mediatorMock.Object, _loggerMock.Object, _sessionServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };
            urlHelper = new Mock<IUrlHelper>();

            UrlRouteContext verifyRouteValues = null;
            urlHelper
               .Setup(m => m.RouteUrl(It.Is<UrlRouteContext>(c =>
                   c.RouteName.Equals(RouteNames.GetStandardDetails)
               )))
               .Returns(verifyUrl)
               .Callback<UrlRouteContext>(c =>
               {
                   verifyRouteValues = c;
               });
            _sut.Url = urlHelper.Object;
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToRoute(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            _mediatorMock
               .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
               .ReturnsAsync(queryResult);

            var result =  await _sut.ConfirmedProviderCourseLocations(model);
            var redirectResult = result as RedirectToRouteResult;
            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(RouteNames.GetStandardDetails);
            redirectResult.RouteValues["ukprn"].Should().Be(int.Parse(Ukprn));
        }

        [Test, AutoData]
        public async Task Post_InValidModel_ReturnsSameView(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);

            queryResult.ProviderCourseLocations = new System.Collections.Generic.List<Domain.ApiModels.ProviderCourseLocation>();

            _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

            var result =  await _sut.ConfirmedProviderCourseLocations(model);

            var viewResult = result as ViewResult;
            viewResult.Should().NotBeNull();
            viewResult.ViewName.Should().Contain("EditTrainingLocations.cshtml");
            viewResult.Model.Should().NotBeNull();
            var modelResult= viewResult.Model as ProviderCourseLocationListViewModel;
            modelResult.Should().NotBeNull();
            modelResult.BackUrl.Should().Be(refererUrl);
            modelResult.CancelUrl.Should().Be(verifyUrl);
        }

        [Test, AutoData]
        public async Task Post_ValidModel_RedirectToNationalDeliveryOption(ProviderCourseLocationListViewModel model, GetProviderCourseLocationsQueryResult queryResult)
        {
            var refererUrl = "http://test-referer-url/";
            _sut.HttpContext.Request.Headers.Add("Referer", refererUrl);

            _mediatorMock
           .Setup(m => m.Send(It.Is<GetProviderCourseLocationsQuery>(q => q.Ukprn == int.Parse(Ukprn) && q.LarsCode == model.LarsCode), It.IsAny<CancellationToken>()))
           .ReturnsAsync(queryResult);

            _sessionServiceMock.Setup(s => s.Get(SessionKeys.SelectedLocationOption, model.LarsCode.ToString())).Returns(LocationOption.Both.ToString());
            var result = await _sut.ConfirmedProviderCourseLocations(model);

            var redirectResult = result as RedirectToRouteResult;

            redirectResult.Should().NotBeNull();
            redirectResult.RouteName.Should().Be(RouteNames.GetNationalDeliveryOption);
            redirectResult.RouteValues["ukprn"].Should().Be(int.Parse(Ukprn));
        }
    }
}
