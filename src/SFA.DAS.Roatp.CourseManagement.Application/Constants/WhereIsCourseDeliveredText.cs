namespace SFA.DAS.Roatp.CourseManagement.Application.Constants
{
    public static class WhereIsCourseDeliveredText
    {
        public const string ProvidersOnly = "This standard is only delivered at your training locations.";
        public const string SubregionsOnly = "This standard is only delivered at an employer's address.";
        public const string NationalOnly = "This standard is only delivered at an employer's address anywhere in England.";
        public const string ProvidersAndNationalOnly = "This standard can be delivered at both training sites and employer addresses anywhere in England.";
        public const string ProvidersAndSubregionsOnly = "This standard can be delivered at both training sites and employer addresses within certain regions.";
        public const string NoneSet = "This standard has no locations associated with it.";
    }
}