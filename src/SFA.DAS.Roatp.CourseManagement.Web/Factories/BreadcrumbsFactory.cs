using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.CourseManagement.Web.Infrastructure;
using SFA.DAS.Roatp.CourseManagement.Web.Models;

namespace SFA.DAS.Roatp.CourseManagement.Web.Factories;

public static class BreadcrumbsFactory
{
    public static BreadcrumbsViewModel Create(IUrlHelper urlHelper, BreadcrumbsFactoryModel model)
    {
        var breadcrumbs = new BreadcrumbsViewModel();

        foreach (var breadcrumbName in model.BreadcrumbNames)
        {
            breadcrumbs.Add(breadcrumbName, GetUrl(urlHelper, breadcrumbName, model));
        }

        return breadcrumbs;
    }

    private static string GetUrl(IUrlHelper urlHelper, string breadcrumbName, BreadcrumbsFactoryModel model)
    {
        return breadcrumbName switch
        {
            BreadcrumbNames.ProviderDashboard => model.DashboardUrl,

            BreadcrumbNames.ManageYourCourses => urlHelper.RouteUrl(RouteNames.ReviewYourDetails, new { model.Ukprn }) ?? string.Empty,

            BreadcrumbNames.ManageApprenticeships => urlHelper.RouteUrl(RouteNames.ViewStandards, new { model.Ukprn }) ?? string.Empty,

            BreadcrumbNames.ManageShortCourses => urlHelper.RouteUrl(RouteNames.ManageShortCourses, new { model.Ukprn, model.LearningType }) ?? string.Empty,

            _ => throw new ArgumentException($"Unknown breadcrumb: {breadcrumbName}", nameof(breadcrumbName))
        } ?? string.Empty;
    }
}
