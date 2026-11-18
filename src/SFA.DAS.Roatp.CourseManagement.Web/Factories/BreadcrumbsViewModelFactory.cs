using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Roatp.CourseManagement.Web.Common;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Factories;

public class BreadcrumbsViewModelFactory(IOptions<ProviderSharedUIConfiguration> providerConfiguration) : IBreadcrumbsViewModelFactory
{
    public BreadcrumbsViewModel Create(IUrlHelper urlHelper, BreadcrumbsViewModelFactoryModel model)
    {
        var breadcrumbs = new BreadcrumbsViewModel();

        foreach (var breadcrumbName in model.BreadcrumbNames)
        {
            breadcrumbs.Add(breadcrumbName, GetUrl(urlHelper, breadcrumbName, model));
        }

        return breadcrumbs;
    }

    private string GetUrl(IUrlHelper urlHelper, string breadcrumbName, BreadcrumbsViewModelFactoryModel model)
    {
        return breadcrumbName switch
        {
            BreadcrumbNames.ProviderDashboard => providerConfiguration.Value.DashboardUrl.TrimEnd('/') + "/account",

            BreadcrumbNames.ManageYourCourses => urlHelper.RouteUrl(RouteNames.ReviewYourDetails, new { model.Ukprn }) ?? string.Empty,

            BreadcrumbNames.ManageApprenticeships => urlHelper.RouteUrl(RouteNames.ViewStandards, new { model.Ukprn }) ?? string.Empty,

            BreadcrumbNames.ManageShortCourses => urlHelper.RouteUrl(RouteNames.ManageShortCourses, new { model.Ukprn, model.LearningType }) ?? string.Empty,

            _ => throw new ArgumentException($"Unknown breadcrumb: {breadcrumbName}", nameof(breadcrumbName))
        } ?? string.Empty;
    }
}
