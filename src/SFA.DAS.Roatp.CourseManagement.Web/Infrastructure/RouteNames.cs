using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class RouteNames
    {
        public const string ProviderSignOut = "provider-signout";
        public const string ReviewYourDetails = "review-your-details";
        public const string ViewStandards = "standards";
        public const string ViewProviderLocations = "traininglocations";
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
    }
}
