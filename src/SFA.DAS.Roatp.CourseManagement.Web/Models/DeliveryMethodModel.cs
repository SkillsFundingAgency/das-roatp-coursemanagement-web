namespace SFA.DAS.Roatp.CourseManagement.Web.Models
{
    public class DeliveryMethodModel
    {
        public const string DayAndBlockRelease = "Day and block release";
        public const string DayRelease = "Day release";
        public const string BlockRelease = "Block release";
        public bool? HasDayReleaseDeliveryOption { get; set; }
        public bool? HasBlockReleaseDeliveryOption { get; set; }
        public string ToSummary()
        {
            if (HasDayReleaseDeliveryOption.GetValueOrDefault() && HasBlockReleaseDeliveryOption.GetValueOrDefault())
            {
                return DayAndBlockRelease;
            }
            if (HasDayReleaseDeliveryOption.GetValueOrDefault())
            {
                return DayRelease;
            }
            if (HasBlockReleaseDeliveryOption.GetValueOrDefault())
            {
                return BlockRelease;
            }
            return string.Empty;
        }
    }
}
