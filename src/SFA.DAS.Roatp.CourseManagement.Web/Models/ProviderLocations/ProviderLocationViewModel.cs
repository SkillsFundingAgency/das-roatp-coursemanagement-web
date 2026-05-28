using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.ProviderLocations
{
    public class ProviderLocationViewModel : ProviderLocationDetailsSubmitModel, IBackLink
    {
        public Guid NavigationId { get; set; }
        public int? RegionId { get; set; }
        public string VenueNameUrl { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string UpdateContactDetailsUrl { get; set; }
        public string DeleteLocationUrl { get; set; }
        public string ManageYourStandardsUrl { get; set; }
        public string TrainingVenuesUrl { get; set; }

        public List<ProviderLocationStandardModel> Standards { get; set; }
        public List<ProviderLocationStandardModel> ApprenticeshipUnits { get; set; }
        public bool HasCourses { get; set; }
        public bool ShowStandards { get; set; }
        public bool ShowApprenticeshipUnits { get; set; }

        public List<string> AddressDetails
        {
            get
            {
                var addressDetails = new List<string>();
                if (!string.IsNullOrWhiteSpace(AddressLine1)) addressDetails.Add(AddressLine1);
                if (!string.IsNullOrWhiteSpace(AddressLine2)) addressDetails.Add(AddressLine2);
                if (!string.IsNullOrWhiteSpace(Town)) addressDetails.Add(Town);
                if (!string.IsNullOrWhiteSpace(Postcode)) addressDetails.Add(Postcode);

                return addressDetails;
            }
        }

        public static implicit operator ProviderLocationViewModel(ProviderLocation source)
        {
            var standards = source.Standards is { Count: > 0 }
                ? source.Standards.Select(s => (ProviderLocationStandardModel)s).Where(s => s.LearningType == Domain.ApiModels.ApprenticeshipType.Apprenticeship).OrderBy(s => s.CourseDisplayName).ToList()
                : [];

            var apprenticeshipUnits = source.Standards is { Count: > 0 }
                ? source.Standards.Select(s => (ProviderLocationStandardModel)s).Where(s => s.LearningType == Domain.ApiModels.ApprenticeshipType.ApprenticeshipUnit).OrderBy(s => s.CourseDisplayName).ToList()
                : [];

            return new ProviderLocationViewModel
            {
                NavigationId = source.NavigationId,
                RegionId = source.RegionId,
                LocationName = source.LocationName,
                AddressLine1 = source.AddressLine1,
                AddressLine2 = source.AddressLine2,
                Town = source.Town,
                County = source.County,
                Postcode = source.Postcode,
                Standards = standards,
                ApprenticeshipUnits = apprenticeshipUnits,
                HasCourses = source.Standards?.Count > 0,
                ShowStandards = standards.Count > 0,
                ShowApprenticeshipUnits = apprenticeshipUnits.Count > 0
            };
        }
    }
}
