using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation
{
    public class AddressViewModel : AddressSubmitModel
    {
        public string ChangeLink { get; set; }
        public string BackLink { get; set; }
        public string CancelLink { get; set; }
        public string Postcode { get; set; }
        public List<SelectListItem> Addresses { get; set; } = new List<SelectListItem>();

        public string AddressesHint => Addresses.Count > 0 ? $"{Addresses.Count} addresses found" : "No address found";
    }

    public class AddressSubmitModel
    {
        public string SelectedAddressUprn { get; set; }
    }
}
