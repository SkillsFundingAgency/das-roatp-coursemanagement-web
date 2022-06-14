using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Standard.Queries;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests
{
    [TestFixture]
    public class EditLocationOptionControllerGetTests
    {
        private Mock<ILogger<EditLocationOptionController>> _loggerMock;
        private Mock<IMediator> _mediatorMock;
        private EditLocationOptionController _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _loggerMock = new Mock<ILogger<EditLocationOptionController>>();
            _mediatorMock = new Mock<IMediator>();

            _sut = new EditLocationOptionController(_mediatorMock.Object, _loggerMock.Object);
            _sut
                .AddDefaultContextWithUser()
                .AddDefaultUrlMock(RouteNames.ViewStandardDetails);
        }

        [Test, AutoData]
        public async Task Get_BackLinkIsSetToStandardDetails()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().BeNull();
            model.BackLink.Should().Be(TestConstants.DefaultUrl);
        }

        [Test, AutoData]
        public async Task Get_CancelLinkIsSetToStandardDetails()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().BeNull();
            model.CancelLink.Should().Be(TestConstants.DefaultUrl);
        }

        [Test]
        public async Task Get_NoCourseLocation_ResturnNullLocationOption()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().BeNull();
        }

        [Test]
        public async Task Get_WithProviderLocationOnly_ReturnsProviderLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { LocationType = LocationType.Provider } };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().Be(LocationOption.ProviderLocation);
        }

        [Test]
        public async Task Get_WithProviderAndNationalLocation_ReturnsBothLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation> 
            { 
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.National },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().Be(LocationOption.Both);
        }

        [Test]
        public async Task Get_WithProviderAndRegionalLocation_ReturnsBothLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.Regional },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().Be(LocationOption.Both);
        }

        [Test]
        public async Task Get_WithRegionalLocationOnly_ReturnsEmployerLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation { LocationType = LocationType.Regional },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        [Test]
        public async Task Get_WithNationalLocationOnly_ReturnsEmployerLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation { LocationType = LocationType.National },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(123);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        private void SetProviderCourseLocationsInMediatorResponse(List<ProviderCourseLocation> providerCourseLocations)
        {
            var standardDetails = new StandardDetails()
            {
                ProviderCourseLocations = providerCourseLocations
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetStandardDetailsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetStandardDetailsQueryResult { StandardDetails = standardDetails });
        }
    }
}
