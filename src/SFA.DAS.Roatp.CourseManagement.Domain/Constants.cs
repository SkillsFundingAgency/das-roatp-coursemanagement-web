namespace SFA.DAS.Roatp.CourseManagement.Domain
{
    public static class Constants
    {
        public static class RegularExpressions
        {
            public const string UrlRegex = @"^(?:[Hh][Tt][Tt][Pp]([Ss])?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$";
            public const string EmailRegex = @"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z0-9_](-?[a-zA-Z0-9_])*(\.[a-zA-Z0-9](-?[a-zA-Z0-9])*)+$";
            public const string AlphaNumericRegex = @"^[\w\d-][\s?[\w\d-]*]*$";
            public const string ExcludedCharactersRegex = @"^[^!""£$%^&*=?/<>;]*$";
        }
    }
}
