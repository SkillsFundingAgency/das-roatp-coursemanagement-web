using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation
{
    public class ProviderLocationDetailsViewModel : ProviderLocationDetailsSubmitModel, IBackLink
    {
        public ProviderLocationDetailsViewModel(AddressItem addressItem)
        {
            AddressLine1 = addressItem.AddressLine1;
            AddressLine2 = addressItem.AddressLine2;
            Town = addressItem.Town;
            Postcode = addressItem.Postcode;
        }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }

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
    }

    public class ProviderLocationDetailsSubmitModel
    {
        public string LocationName { get; set; }
    }
}
