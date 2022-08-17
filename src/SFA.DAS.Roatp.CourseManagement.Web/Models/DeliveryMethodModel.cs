namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class DeliveryMethodModel
    {
        public const string DayAndBlockReleaseDescription = "Day and block release";
        public const string DayReleaseDescription = "Day release";
        public const string BlockReleaseDescription = "Block release";
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public string ToSummary()
        {
            if (HasDayReleaseDeliveryOption.GetValueOrDefault() && HasBlockReleaseDeliveryOption.GetValueOrDefault())
            {
                return DayAndBlockReleaseDescription;
            }
            if (HasDayReleaseDeliveryOption.GetValueOrDefault())
            {
                return DayReleaseDescription;
            }
            if (HasBlockReleaseDeliveryOption.GetValueOrDefault())
            {
                return BlockReleaseDescription;
            }
            return string.Empty;
        }
    }
}
