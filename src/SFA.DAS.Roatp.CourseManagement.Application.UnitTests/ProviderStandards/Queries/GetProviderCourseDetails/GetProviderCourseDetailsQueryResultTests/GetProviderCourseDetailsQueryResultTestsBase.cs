using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetProviderCourseDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetProviderCourseDetails.GetProviderCourseDetailsQueryResultTests
{
    public abstract class GetProviderCourseDetailsQueryResultTestsBase
    {
        protected readonly ProviderCourseLocation _providerLocation = new ProviderCourseLocation { LocationType = LocationType.Provider };
        protected readonly ProviderCourseLocation _nationalLocation = new ProviderCourseLocation { LocationType = LocationType.National };
        protected readonly ProviderCourseLocation _regionalLocation = new ProviderCourseLocation { LocationType = LocationType.Regional };

        protected GetProviderCourseDetailsQueryResult _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _sut = new GetProviderCourseDetailsQueryResult
            {
                ProviderCourseLocations = new List<ProviderCourseLocation>
                { _providerLocation, _nationalLocation, _regionalLocation }
            };
        }
    }

}
