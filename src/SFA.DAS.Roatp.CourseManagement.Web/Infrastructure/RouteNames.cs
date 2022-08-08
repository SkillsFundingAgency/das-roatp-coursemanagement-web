﻿using System.Diagnostics.CodeAnalysis;
using SFA.DAS.Roatp.CourseManagement.Web.Models.Standards;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RouteNames
    {
        public const string ProviderSignOut = "provider-signout";
        public const string ReviewYourDetails = "review-your-details";
        public const string ViewStandards = "standards";
        public const string GetProviderLocations = "traininglocations";
        public const string GetStandardDetails = "GetStandardDetails";
        public const string GetCourseContactDetails = "GetCourseContactDetails";
        public const string PostCourseContactDetails = "PostCourseContactDetails";
        public const string GetConfirmRegulatedStandard = "GetConfirmRegulatedStandard";
        public const string PostConfirmRegulatedStandard = "PostConfirmRegulatedStandard";
        public const string GetLocationOption = "GetLocationOption";
        public const string PostLocationOption = "PostLocationOption";
        public const string GetStandardSubRegions = "GetStandardSubRegions";
        public const string PostStandardSubRegions = "PostStandardSubRegions";
        public const string GetNationalDeliveryOption = "GetNationalDeliveryOption";
        public const string PostNationalDeliveryOption = "PostNationalDeliveryOption";
        public const string GetProviderCourseLocations = "GetProviderCourseLocations";
        public const string PostProviderCourseLocations = "PostProviderCourseLocations";
        public const string GetProviderLocationDetails = "GetProviderLocationDetails";
        public const string GetRemoveProviderCourseLocation = "GetRemoveProviderCourseLocation";
        public const string PostRemoveProviderCourseLocation = "PostRemoveProviderCourseLocation";

        public const string GetAddStandardSelectStandard = "GetAddStandardSelectStandard";
        public const string PostAddStandardSelectStandard = "PostAddStandardSelectStandard";
        public const string GetAddStandardConfirmNonRegulatedStandard = "GetAddStandardConfirmNonRegulatedStandard";
        public const string PostAddStandardConfirmNonRegulatedStandard = "PostAddStandardConfirmNonRegulatedStandard";
        public const string GetAddStandardAddContactDetails = "GetAddStandardAddContactDetails";
        public const string PostAddStandardAddContactDetails = "PostAddStandardAddContactDetails";
        public const string GetAddStandardSelectLocationOption = "GetAddStandardSelectLocationOption";
        public const string PostAddStandardSelectLocationOption = "PostAddStandardSelectLocationOption";

        public const string GetTrainingLocationPostcode = "GetTrainingLocationPostcode";
        public const string PostTrainingLocationPostcode = "PostTrainingLocationPostcode";
        public const string GetProviderLocationAddress = "GetTrainingLocationAddress";
        public const string PostProviderLocationAddress = "PostTrainingLocationAddress";

        public const string GetAddProviderLocationDetails = "GetAddProviderLocationDetails";
        public const string PostAddProviderLocationDetails = "PostAddProviderLocationDetails";

        public const string GetUpdateProviderLocationDetails = "GetUpdateProviderLocationDetails";
        public const string PostUpdateProviderLocationDetails = "PostUpdateProviderLocationDetails";
        public const string ProviderDescription = "provider-description";
    }
}
