using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Common;
using SFA.DAS.Roatp.CourseManagement.Web.Factories;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Factories;

public class BreadcrumbsViewModelFactoryTests
{
    private const int Ukprn = 12345678;
    private const string DashboardUrl = "https://test/dashboard";
    private const string ReviewYourDetailsUrl = "https://test/review-your-details";
    private const string ManageApprenticeshipsUrl = "https://test/manage-apprenticeships";
    private const string ManageShortCoursesUrl = "https://test/manage-short-courses";
    private BreadcrumbsViewModelFactory _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new BreadcrumbsViewModelFactory(
            Options.Create(new ProviderSharedUIConfiguration
            {
                DashboardUrl = DashboardUrl
            }));
    }

    [Test, MoqAutoData]
    public void WhenProviderDashboardBreadcrumbIsRequested_ThenBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames = [BreadcrumbNames.ProviderDashboard]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl)
            },
            options => options.WithStrictOrdering());
    }

    [Test, MoqAutoData]
    public void WhenManageYourCoursesBreadcrumbIsRequested_ThenBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        SetupRouteUrl(urlHelper, RouteNames.ReviewYourDetails, ReviewYourDetailsUrl);

        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames = [BreadcrumbNames.ManageYourCourses]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageYourCourses,
                    ReviewYourDetailsUrl)
            });
    }

    [Test, MoqAutoData]
    public void WhenManageApprenticeshipsBreadcrumbIsRequested_ThenBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        SetupRouteUrl(urlHelper, RouteNames.ViewStandards, ManageApprenticeshipsUrl);

        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames = [BreadcrumbNames.ManageApprenticeships]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageApprenticeships,
                    ManageApprenticeshipsUrl)
            });
    }

    [Test, MoqAutoData]
    public void WhenManageShortCoursesBreadcrumbIsRequested_ThenBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        SetupRouteUrl(urlHelper, RouteNames.ManageShortCourses, ManageShortCoursesUrl);

        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            LearningType = LearningType.ApprenticeshipUnit,
            BreadcrumbNames = [BreadcrumbNames.ManageShortCourses]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageShortCourses,
                    ManageShortCoursesUrl)
            });
    }

    [Test, MoqAutoData]
    public void WhenMultipleBreadcrumbsAreRequested_ThenBreadcrumbsAreCreatedInOrder(
        Mock<IUrlHelper> urlHelper)
    {
        SetupRouteUrl(urlHelper, RouteNames.ReviewYourDetails, ReviewYourDetailsUrl);
        SetupRouteUrl(urlHelper, RouteNames.ViewStandards, ManageApprenticeshipsUrl);

        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ProviderDashboard,
                BreadcrumbNames.ManageYourCourses,
                BreadcrumbNames.ManageApprenticeships
            ]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEquivalentTo(
            new[]
            {
                new BreadcrumbItemModel(
                    BreadcrumbNames.ProviderDashboard,
                    DashboardUrl),
                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageYourCourses,
                    ReviewYourDetailsUrl),
                new BreadcrumbItemModel(
                    BreadcrumbNames.ManageApprenticeships,
                    ManageApprenticeshipsUrl)
            },
            options => options.WithStrictOrdering());
    }

    [Test, MoqAutoData]
    public void WhenNoBreadcrumbsAreRequested_ThenNoBreadcrumbsAreCreated(
        Mock<IUrlHelper> urlHelper)
    {
        var model = new BreadcrumbsViewModelFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames = []
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public void WhenRequiredRouteValuesAreNotPassed_ThenShouldReturnEmptyUrl(
        Mock<IUrlHelper> urlHelper)
    {
        urlHelper
            .Setup(x => x.RouteUrl(It.Is<UrlRouteContext>(
                context => context.RouteName == RouteNames.ManageShortCourses)))
            .Returns((string)null);

        var model = new BreadcrumbsViewModelFactoryModel
        {
            BreadcrumbNames = [BreadcrumbNames.ManageShortCourses]
        };

        var result = _sut.Create(urlHelper.Object, model);

        result.Items.Should().HaveCount(1);
        result.Items[0].Url.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public void WhenUnknownBreadcrumbIsRequested_ThenArgumentExceptionIsThrown(
        Mock<IUrlHelper> urlHelper)
    {
        var model = new BreadcrumbsViewModelFactoryModel
        {
            BreadcrumbNames = ["Unknown breadcrumb"]
        };

        var act = () => _sut.Create(urlHelper.Object, model);

        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*Unknown breadcrumb*");
    }

    private static void SetupRouteUrl(Mock<IUrlHelper> urlHelper, string routeName, string url)
    {
        urlHelper
            .Setup(x => x.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == routeName)))
            .Returns(url);
    }
}
