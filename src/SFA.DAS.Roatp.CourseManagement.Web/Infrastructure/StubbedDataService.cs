using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure
{
    public static class StubbedDataService
    {
        public static StandardDetails GetStubbedData(string ukprn, int larsCode)
        {
            var ukprnToMatch = "10019026";

            if (ukprnToMatch != ukprn)
                return null;

            const int larsCodeProvidersOnly = 1;
            const int larsCodeSubRegionsOnly = 2;
            const int larsCodeNationalOnly = 3;
            const int larsCodeProvidersAndSubregionsOnly = 4;
            const int larsCodeProvidersAndNationalOnly = 5;
           
            switch (larsCode)
            {
                case (larsCodeProvidersOnly):
                    return GetProvidersOnly(larsCode);
                case (larsCodeSubRegionsOnly):
                    return GetSubregionsOnly(larsCode);
                case (larsCodeNationalOnly):
                    return GetNationalOnly(larsCode);
                case (larsCodeProvidersAndSubregionsOnly):
                    return GetProvidersAndSubregions(larsCode);
                case (larsCodeProvidersAndNationalOnly):
                    return GetProvidersAndNational(larsCode);
                default: 
                    return null;
            }
        }

        private static StandardDetails GetProvidersOnly(int larsCode)
        {
            var view = GetStandardDetails(larsCode);
            view.ProviderCourseLocations = GetProvidersData();
            
            return view;
        }

        private static StandardDetails GetSubregionsOnly(int larsCode)
        {
            var view = GetStandardDetails(larsCode);
            view.ProviderCourseLocations = GetRegionalData();

            return view;
        }

        private static StandardDetails GetNationalOnly(int larsCode)
        {
            var view = GetStandardDetails(larsCode);
            view.ProviderCourseLocations = GetNationalData();

            return view;
        }

        private static StandardDetails GetProvidersAndSubregions(int larsCode)
        {
            var view = GetStandardDetails(larsCode);
            view.ProviderCourseLocations = GetProvidersData();
            view.ProviderCourseLocations.AddRange(GetRegionalData());
            return view;
        }

        private static StandardDetails GetProvidersAndNational(int larsCode)
        {
            var view = GetStandardDetails(larsCode);
            view.ProviderCourseLocations = GetProvidersData();
            view.ProviderCourseLocations.AddRange(GetNationalData());
            return view;
        }

        private static List<ProviderCourseLocation> GetProvidersData()
        {
            var data = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation
                {
                    LocationName = "office 1",
                    LocationType = LocationType.Provider,
                    HasDayReleaseDeliveryOption = null,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false
                },
                new ProviderCourseLocation
                {
                    LocationName = "office 2",
                    LocationType = LocationType.Provider,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = false,
                    OffersPortableFlexiJob = true
                },
                new ProviderCourseLocation
                {
                    LocationName = "office 3",
                    LocationType = LocationType.Provider,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false
                },
            };

            
            return data;
        }

        private static List<ProviderCourseLocation> GetNationalData()
        {
            var data = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation
                {
                    LocationName = "National",
                    LocationType = LocationType.National,
                    HasDayReleaseDeliveryOption = null,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false
                }
            };

            return data;

        }

        private static List<ProviderCourseLocation> GetRegionalData()
        {
            var data = new List<ProviderCourseLocation>
            {
                new ProviderCourseLocation
                {
                    LocationName = "Bedford",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = null,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false,
                    RegionName = "East of England"
                },
                new ProviderCourseLocation
                {
                    LocationName = "Bath and North East Somerset",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = false,
                    OffersPortableFlexiJob = true,
                    RegionName="South West"
                },
                new ProviderCourseLocation
                {
                    LocationName = "Swindon",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = false,
                    OffersPortableFlexiJob = true,
                    RegionName="South West"
                },
                new ProviderCourseLocation
                {
                    LocationName = "Derby",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false,
                    RegionName="East Midlands"
                },
                new ProviderCourseLocation
                {
                    LocationName = "Buckinghamshire",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = null,
                    HasBlockReleaseDeliveryOption = true,
                    OffersPortableFlexiJob = false,
                    RegionName="South East"
                },
                new ProviderCourseLocation
                {
                    LocationName = "Bristol",
                    LocationType = LocationType.Regional,
                    HasDayReleaseDeliveryOption = true,
                    HasBlockReleaseDeliveryOption = false,
                    OffersPortableFlexiJob = true,
                    RegionName="South West"
                }
            };

            return data;
        }

        private static StandardDetails GetStandardDetails(int larsCode)
        {
            var regulatorName = "";
            if (larsCode == 1)
                regulatorName = "Regulator name";

            var standard = new StandardDetails
            {
                CourseName = $"test1 - {larsCode}",
                Level = "1",
                IFateReferenceNumber = "1234",
                Sector = "Digital",
                LarsCode = larsCode,
                RegulatorName = regulatorName,
                Version = "1.1",
                StandardInfoUrl = "www.test.com",
                ContactUsEmail = "test@test.com",
                ContactUsPageUrl = "www.test.com/ContactUs",
                ContactUsPhoneNumber = "123456789"
            };
            return standard;
        }
    }
}
