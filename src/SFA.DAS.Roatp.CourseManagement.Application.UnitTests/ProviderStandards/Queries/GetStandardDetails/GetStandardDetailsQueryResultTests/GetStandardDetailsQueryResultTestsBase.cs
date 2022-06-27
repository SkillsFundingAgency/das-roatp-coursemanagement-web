using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.ProviderStandards.Queries.GetStandardDetails;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.ProviderStandards.Queries.GetStandardDetails.GetStandardDetailsQueryResultTests
{
    public abstract class GetStandardDetailsQueryResultTestsBase
    {
        protected readonly ProviderCourseLocation _providerLocation = new ProviderCourseLocation { LocationType = LocationType.Provider };
        protected readonly ProviderCourseLocation _nationalLocation = new ProviderCourseLocation { LocationType = LocationType.National };
        protected readonly ProviderCourseLocation _regionalLocation = new ProviderCourseLocation { LocationType = LocationType.Regional };

        protected GetStandardDetailsQueryResult _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _sut = new GetStandardDetailsQueryResult
            {
                ProviderCourseLocations = new List<ProviderCourseLocation>
                { _providerLocation, _nationalLocation, _regionalLocation }
            };
        }
    }

}
