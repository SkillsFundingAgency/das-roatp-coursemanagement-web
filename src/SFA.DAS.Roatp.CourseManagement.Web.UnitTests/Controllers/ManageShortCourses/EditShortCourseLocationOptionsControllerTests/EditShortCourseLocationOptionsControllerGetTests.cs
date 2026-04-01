using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.ManageShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ShortCourses.AddAShortCourse;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.ManageShortCourses.EditShortCourseLocationOptionsControllerTests;
public class EditShortCourseLocationOptionsControllerGetTests
{
    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderCourseExists_ReturnsView(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseLocationOptionsController sut,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        List<ShortCourseLocationOptionModel> locationOptions = new()
        {
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.ProviderLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.EmployerLocation, IsSelected = false },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.Online, IsSelected = false },
        };

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult();

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);


        // Act
        var result = await sut.EditShortCourseLocationOptions(apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as SelectShortCourseLocationOptionsViewModel;
        model.LocationOptions.Should().BeEquivalentTo(locationOptions);
        model.ApprenticeshipType.Should().Be(apprenticeshipType);
        model.SubmitButtonText.Should().Be(ButtonText.Confirm);
        model.Route.Should().Be(RouteNames.EditShortCourseLocationOptions);
        model.IsAddJourney.Should().BeFalse();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderCourseExists_IsSelectedIsSetCorrectly(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseLocationOptionsController sut,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;

        List<ShortCourseLocationOptionModel> locationOptions = new()
        {
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.ProviderLocation, IsSelected = true },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.EmployerLocation, IsSelected = true },
            new ShortCourseLocationOptionModel { LocationOption = ShortCourseLocationOption.Online, IsSelected = true },
        };

        var providerCourseDetailsApiResponse = new GetProviderCourseDetailsQueryResult()
        {
            HasOnlineDeliveryOption = true,
            ProviderCourseLocations = new List<ProviderCourseLocation>()
            {
                new ProviderCourseLocation { LocationType = LocationType.Provider },
                new ProviderCourseLocation { LocationType = LocationType.National },
                new ProviderCourseLocation { LocationType = LocationType.Regional }
            }
        };

        providerCourseDetailsApiResponse.HasOnlineDeliveryOption = true;

        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);


        // Act
        var result = await sut.EditShortCourseLocationOptions(apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        var model = viewResult.Model as SelectShortCourseLocationOptionsViewModel;
        model.LocationOptions.FirstOrDefault().IsSelected.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderCourseExists_VerifyMediatorInvokedCorrectly(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseLocationOptionsController sut,
    GetProviderCourseDetailsQueryResult providerCourseDetailsApiResponse,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;


        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(providerCourseDetailsApiResponse);


        // Act
        await sut.EditShortCourseLocationOptions(apprenticeshipType, larscode);

        // Assert
        mediatorMock.Verify(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task EditShortCourseLocationOptions_ProviderCourseDoesNotExist_RedirectToPageNotFound(
    [Frozen] Mock<IMediator> mediatorMock,
    [Greedy] EditShortCourseLocationOptionsController sut,
    string larscode
)
    {
        // Arrange
        var apprenticeshipType = ApprenticeshipType.ApprenticeshipUnit;


        sut.AddDefaultContextWithUser();

        mediatorMock.Setup(m => m.Send(It.Is<GetProviderCourseDetailsQuery>(q => q.Ukprn.ToString() == TestConstants.DefaultUkprn && q.LarsCode == larscode), It.IsAny<CancellationToken>())).ReturnsAsync(() => null);


        // Act
        var result = await sut.EditShortCourseLocationOptions(apprenticeshipType, larscode);

        // Assert
        var viewResult = result as ViewResult;
        viewResult.Should().NotBeNull();
        viewResult!.ViewName.Should().Be(ViewsPath.PageNotFoundPath);
    }
}
