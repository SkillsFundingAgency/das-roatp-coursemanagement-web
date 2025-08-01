using System;

namespace SFA.DAS.Roatp.CourseManagement.Web.Services
{
    public static class LocationSummaryCalculator
    {
        public const string ProvidersOnly = "This standard is only delivered at your training locations.";
        public const string SubregionsOnly = "This standard is only delivered at an employer's address.";
        public const string NationalOnly = "This standard is only delivered at an employer's address anywhere in England.";
        public const string ProvidersAndNational = "This standard can be delivered at both training venues and employer addresses anywhere in England.";
        public const string ProvidersAndSubregions = "This standard can be delivered at both training venues and employer addresses within certain regions.";
        public const string NoneSet = "No training options. Select <strong>Change</strong> to add one.";
        public const string ErrorMessage = "The combination of national and regional is invalid, only one of them can be true";

        public static string GetLocationSummary(bool hasNationalLocation, bool hasProviderLocation, bool hasRegionalLocation)
        {
            if (hasNationalLocation && hasRegionalLocation)
            {
                throw new ArgumentException(ErrorMessage);
            }

            if (hasProviderLocation)
            {
                if (hasNationalLocation)
                    return ProvidersAndNational;

                if (hasRegionalLocation)
                    return ProvidersAndSubregions;

                return ProvidersOnly;
            }

            if (hasNationalLocation)
                return NationalOnly;

            if (hasRegionalLocation)
            {
                return SubregionsOnly;
            }

            return NoneSet;
        }
    }
}
