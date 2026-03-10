using System.Collections.Generic;
using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Domain.Models.Constants;
using SFA.DAS.Roatp.CourseManagement.Web.Controllers.AddProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderContact;
using SFA.DAS.Roatp.CourseManagement.Web.Services;
using SFA.DAS.Roatp.CourseManagement.Web.UnitTests.TestHelpers;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Controllers.AddProviderContactTests;

[TestFixture]
public class SelectStandardsForUpdateControllerGetTests
{
    [Test, MoqAutoData]
    public void Get_ModelMissingFromSession_RedirectsToReviewYourDetails(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectStandardsForUpdateController sut,
        int ukprn)
    {
        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns((ProviderContactSessionModel)null);

        var result = sut.SelectStandards(ukprn);

        var redirectResult = result as RedirectToRouteResult;

        redirectResult!.RouteName.Should().Be(RouteNames.ReviewYourDetails);

        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_PopulatesExpectedModel(
        [Frozen] Mock<ISessionService> sessionServiceMock,
        [Greedy] SelectStandardsForUpdateController sut,
        List<ProviderContactStandardModel> standards,
        int ukprn
    )
    {
        var email = "test@test.com";
        var phoneNumber = "123445";

        var sessionModel = new ProviderContactSessionModel
        {
            EmailAddress = email,
            PhoneNumber = phoneNumber,
            Standards = standards
        };

        var expectedShowStandards = sessionModel.Standards.Any(x => x.CourseType == CourseType.Apprenticeship);
        var expectedApprenticeshipUnits = sessionModel.Standards.Any(x => x.CourseType == CourseType.ShortCourse);

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.SelectStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactStandardsViewModel;

        model!.Standards.Should().BeEquivalentTo(standards.Where(x => x.CourseType == CourseType.Apprenticeship).ToList(), options => options.Excluding(x => x.CourseName));
        model!.ApprenticeshipUnits.Should().BeEquivalentTo(standards.Where(x => x.CourseType == CourseType.ShortCourse).ToList(), options => options.Excluding(x => x.CourseName));
        model!.ShowStandards.Should().Be(expectedShowStandards);
        model!.ShowApprenticeshipUnits.Should().Be(expectedApprenticeshipUnits);
        sessionServiceMock.Verify(s => s.Get<ProviderContactSessionModel>(), Times.Once);
    }

    [Test, MoqAutoData]
    public void Get_ModelInSession_ExpectedCourseName(
    [Frozen] Mock<ISessionService> sessionServiceMock,
    [Greedy] SelectStandardsForUpdateController sut,
    List<ProviderContactStandardModel> standards,
    int ukprn
)
    {

        var sessionModel = new ProviderContactSessionModel
        {
            Standards = new List<ProviderContactStandardModel>
            {
                new ProviderContactStandardModel
                {
                    CourseName = "test",
                    Level = 1
                }
            }
        };

        sut.AddDefaultContextWithUser();
        sessionServiceMock.Setup(s => s.Get<ProviderContactSessionModel>()).Returns(sessionModel);

        var result = sut.SelectStandards(ukprn);

        var viewResult = result as ViewResult;

        var model = viewResult!.Model as AddProviderContactStandardsViewModel;

        model!.Standards.FirstOrDefault().CourseName.Should().Be($"{sessionModel.Standards.FirstOrDefault().CourseName} (level {sessionModel.Standards.FirstOrDefault().Level})");
    }
}
