using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddAStandard;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddAStandard.UseSavedContactDetailsControllerTests;

[TestFixture]
public class UseSavedContactDetailsControllerGetTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToGetAddStandardSelectStandard(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns((StandardSessionModel)null);

        var result = sut.UseSavedContactDetails(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<StandardSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
    }

    [Test, MoqAutoData]
    public void Get_LatestProviderContactMissingFromSession_RedirectsToGetAddStandardSelectStandard(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(new StandardSessionModel { LatestProviderContactModel = null });

        var result = sut.UseSavedContactDetails(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        sessionServiceMock.Verify(s => s.Get<StandardSessionModel>(), Times.Once);
        redirectResult!.RouteName.Should().Be(RouteNames.GetAddStandardSelectStandard);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] UseSavedContactDetailsController sut,
        StandardSessionModel standardSessionModel,
        int ukprn
    )
    {
        sut.AddDefaultContextWithUser();

        sessionServiceMock.Setup(s => s.Get<StandardSessionModel>()).Returns(standardSessionModel);

        var result = sut.UseSavedContactDetails(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as UseSavedContactDetailsViewModel;
        model!.EmailAddress.Should().Be(standardSessionModel.LatestProviderContactModel.EmailAddress);
        model.PhoneNumber.Should().Be(standardSessionModel.LatestProviderContactModel.PhoneNumber);
        model.ShowEmail.Should().Be(!string.IsNullOrWhiteSpace(standardSessionModel.LatestProviderContactModel.EmailAddress));
        model.ShowPhone.Should()
            .Be(!string.IsNullOrWhiteSpace(standardSessionModel.LatestProviderContactModel.PhoneNumber));
        model.IsUsingSavedContactDetails.Should().Be(standardSessionModel.IsUsingSavedContactDetails);
        sessionServiceMock.Verify(s => s.Get<StandardSessionModel>(), Times.Once);
    }
}