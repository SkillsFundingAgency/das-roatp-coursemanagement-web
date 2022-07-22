using SFA.DAS.Roatp.CourseManagement.Domain.ApiModels;

namespace SFA.DAS.Roatp.CourseManagement.Web.Models.AddTrainingLocation
{
    public class ProviderLocationDetailsViewModel : TrainingLocationDetailsSubmitModel
    {
        public ProviderLocationDetailsViewModel(AddressItem addressItem)
        {
            AddressLine1 = addressItem.AddressLine1;
            AddressLine2 = addressItem.AddressLine2;
            Town = addressItem.Town;
            Postcode = addressItem.Postcode;
        }

        public string BackLink { get; set; } = "#";
        public string CancelLink { get; set; } = "#";
    }

    public class TrainingLocationDetailsSubmitModel
    {
        public string LocationName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
    }
}
