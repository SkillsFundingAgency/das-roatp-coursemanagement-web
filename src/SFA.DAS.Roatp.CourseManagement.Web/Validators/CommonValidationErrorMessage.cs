﻿namespace SFA.DAS.Roatp.CourseManagement.Web.Validators
{
    public static class CommonValidationErrorMessage
    {
        public const string EmailMissingMessage = "Enter an email address";
        public const string EmailLengthMessage = "Email address must be 256 characters or less";
        public const string EmailInvalidMessage = "Enter an email address in the correct format, like name@example.com";

        public const string TelephoneMissingMessage = "Enter a UK telephone number";
        public const string TelephoneLengthMessage = "Telephone number must be between 10 and 50 characters";
        public const string TelephoneHasExcludedCharacter = "Enter a phone number in the correct format";

        public const string WebsiteMissingMessage = "Enter a website link";
        public const string WebsiteLengthMessage = "Website address must be 500 characters or less";
        public const string WebsiteInvalidMessage = "Enter an address in the correct format, like www.example.com";
    }
}
