using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.EditLocationOptionControllerTests
{
    [TestFixture]
    public class EditLocationOptionControllerGetTests : EditLocationOptionControllerTestBase
    {
        private const int LarsCode = 123;
        [Test, AutoData]
        public async Task Get_BackLinkIsSetToStandardDetails()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.None);
        }

        [Test]
        public async Task Get_NoCourseLocation_ResturnNullLocationOption()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.None);
        }

        [Test]
        public async Task Get_DeletesLocationOptionFromSession()
        {
            SetProviderCourseLocationsInMediatorResponse(new List<ProviderCourseLocation>());

            await _sut.Index(LarsCode);

            _sessionServiceMock.Verify(s => s.Delete(SessionKeys.SelectedLocationOption));
        }

        [Test]
        public async Task Get_WithProviderLocationOnly_ReturnsProviderLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation> { new ProviderCourseLocation { LocationType = LocationType.Provider } };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.ProviderLocation);
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

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.Both);
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

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.Both);
        }

        [Test]
        public async Task Get_WithRegionalLocationOnly_ReturnsEmployerLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation { LocationType = LocationType.Regional },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }

        [Test]
        public async Task Get_WithNationalLocationOnly_ReturnsEmployerLocationOption()
        {
            var providerCourseLocations = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation { LocationType = LocationType.National },
            };
            SetProviderCourseLocationsInMediatorResponse(providerCourseLocations);

            var actionResult = await _sut.Index(LarsCode);

            var viewResult = (ViewResult)actionResult;
            Assert.IsNotNull(viewResult);
            var model = (EditLocationOptionViewModel)viewResult.Model;
            model!.LocationOption.Should().Be(LocationOption.EmployerLocation);
        }
    }
}
