namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public static class CommonValidationErrorMessage
    {
        public const string EmailMissingMessage = "You must enter an email address";
        public const string EmailLengthMessage = "Email address must be 256 characters or less";
        public const string EmailInvalidMessage = "Enter an email address in the correct format, like name@example.com";
        public const string EmailInvalidDomainMessage = "Enter an email address with a valid domain";

        public const string TelephoneMissingMessage = "You must enter a phone number";
        public const string TelephoneLengthMessage = "Telephone number must be between 10 and 50 characters";
        public const string TelephoneHasExcludedCharacter = "Enter a phone number in the correct format";

        public const string WebsiteMissingMessage = "You must enter a website";
        public const string WebsiteLengthMessage = "The URL cannot be longer than 500 characters";
        public const string WebsiteInvalidMessage = "Enter a website in the correct format";
    }
}
