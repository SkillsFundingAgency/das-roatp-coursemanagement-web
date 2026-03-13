namespace SFA.DAS.Roatp.CourseManagement.Web.Extensions;

public static class GetNumberSuffixExtension
{
    public static string ToNumberWithSuffix(this int number)
    {
        int remainder = number % 10;
        return remainder switch
        {
            1 => $"{number}st",
            2 => $"{number}nd",
            3 => $"{number}rd",
            _ => $"{number}th"
        };
    }
}
