using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;
using static SFA.DAS.Roatp.CourseManagement.Domain.Constants;

namespace SFA.DAS.Roatp.CourseManagement.Domain.UnitTests.ConstantsTests.RegularExpressionsTests
{
    [TestFixture]
    public class EmailRegexTests
    {
        [TestCase("a.b@c.d")]
        [TestCase("a@c.d")]
        [TestCase("1@2.3")]
        public void EmailRegex_AllowedFormats(string input)
        {
            Regex.Match(input, RegularExpressions.EmailRegex).Success.Should().BeTrue();
        }

        [TestCase("ab.cd")]
        [TestCase("@ab.com")]
        [TestCase("a.b@cd")]
        [TestCase("")]
        [TestCase("  ")]
        public void EmailRegex_InvalidFormats(string input)
        {
            Regex.Match(input, RegularExpressions.EmailRegex).Success.Should().BeFalse();
        }
    }
}
