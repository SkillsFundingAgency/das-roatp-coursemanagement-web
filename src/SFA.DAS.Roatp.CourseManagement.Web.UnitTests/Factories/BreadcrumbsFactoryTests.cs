using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Factories;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.CourseManagement.Web.UnitTests.Factories;

public class BreadcrumbsFactoryTests
{
    private const int Ukprn = 12345678;
    private const string DashboardUrl = "https://test/dashboard";
    private const string ReviewYourDetailsUrl = "https://test/review-your-details";
    private const string ManageApprenticeshipsUrl = "https://test/manage-apprenticeships";
    private const string ManageShortCoursesUrl = "https://test/manage-short-courses";

    [Test, MoqAutoData]
    public void WhenDashboardBreadcrumbIsRequested_ThenDashboardBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        // Arrange
        var model = new BreadcrumbsFactoryModel
        {
            DashboardUrl = DashboardUrl,
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ProviderDashboard
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
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
    public void WhenReviewYourDetailsBreadcrumbIsRequested_ThenBreadcrumbIsCreated(
        Mock<IUrlHelper> urlHelper)
    {
        // Arrange
        SetupRouteUrl(urlHelper, RouteNames.ReviewYourDetails, ReviewYourDetailsUrl);

        var model = new BreadcrumbsFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ManageYourCourses
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
        result.Items.Should().BeEquivalentTo(new[]
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
        // Arrange
        SetupRouteUrl(urlHelper, RouteNames.ViewStandards, ManageApprenticeshipsUrl);

        var model = new BreadcrumbsFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ManageApprenticeships
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert

        result.Items.Should().BeEquivalentTo(new[]
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
        // Arrange
        SetupRouteUrl(urlHelper, RouteNames.ManageShortCourses, ManageShortCoursesUrl);

        var model = new BreadcrumbsFactoryModel
        {
            Ukprn = Ukprn,
            LearningType = LearningType.ApprenticeshipUnit,
            BreadcrumbNames =
            [
                BreadcrumbNames.ManageShortCourses
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
        result.Items.Should().BeEquivalentTo(new[]
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
        // Arrange
        SetupRouteUrl(urlHelper, RouteNames.ReviewYourDetails, ReviewYourDetailsUrl);

        SetupRouteUrl(urlHelper, RouteNames.ViewStandards, ManageApprenticeshipsUrl);

        var model = new BreadcrumbsFactoryModel
        {
            DashboardUrl = DashboardUrl,
            Ukprn = Ukprn,
            BreadcrumbNames =
            [
                BreadcrumbNames.ProviderDashboard,
                BreadcrumbNames.ManageYourCourses,
                BreadcrumbNames.ManageApprenticeships
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
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
        // Arrange
        var model = new BreadcrumbsFactoryModel
        {
            Ukprn = Ukprn,
            BreadcrumbNames = []
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
        result.Items.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public void WhenRequiredRouteValuesAreNotPassed_ThenShouldReturnEmptyUrl(
    Mock<IUrlHelper> urlHelper)
    {
        // Arrange
        urlHelper
            .Setup(x => x.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == RouteNames.ManageShortCourses)))
            .Returns((string)null);

        var model = new BreadcrumbsFactoryModel
        {
            BreadcrumbNames =
            [
                BreadcrumbNames.ManageShortCourses
            ]
        };

        // Act
        var result = BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Url.Should().BeEmpty();
    }

    [Test, MoqAutoData]
    public void WhenUnknownBreadcrumbIsRequested_ThenArgumentExceptionIsThrown(
        Mock<IUrlHelper> urlHelper)
    {
        // Arrange
        var model = new BreadcrumbsFactoryModel
        {
            BreadcrumbNames =
            [
                "Unknown breadcrumb"
            ]
        };

        // Act
        var act = () => BreadcrumbsFactory.Create(urlHelper.Object, model);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*Unknown breadcrumb*");
    }

    private static void SetupRouteUrl(Mock<IUrlHelper> urlHelper, string routeName, string url)
    {
        urlHelper
            .Setup(x => x.RouteUrl(It.Is<UrlRouteContext>(context => context.RouteName == routeName)))
            .Returns(url);
    }
}
