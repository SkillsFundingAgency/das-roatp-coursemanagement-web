using System.Collections.Generic;
using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;
using SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models;

public class ConfirmAddProviderLocationViewModel : ProviderLocationDetailsSubmitModel, IBackLink
{
    public string SubmitButtonText { get; set; }
    public bool ShowCancelOption { get; set; }
    public string Route { get; set; }
    public bool IsAddJourney { get; set; }
    public string DisplayHeader { get; set; }
    public ConfirmAddProviderLocationViewModel(AddressItem addressItem)
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
    public string CancelLink { get; set; } = "#";

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
