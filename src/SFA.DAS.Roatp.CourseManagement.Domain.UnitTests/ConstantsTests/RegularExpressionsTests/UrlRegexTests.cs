using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace SFA.DAS.Roatp.CourseManagement.Domain.UnitTests.ConstantsTests.RegularExpressionsTests
{
    [TestFixture]
    public class UrlRegexTests
    {
        [TestCase("www.goal.co")]
        [TestCase("www.goal.com")]
        [TestCase("goal.com")]
        [TestCase("http://goal.com")]
        [TestCase("https://goal.com")]
        [TestCase("http://www.goal.com")]
        [TestCase("Http://www.goal.com")]
        [TestCase("HTTP://www.goal.com")]
        [TestCase("HTTPS://www.goal.com")]
        [TestCase("www.contactus.service.co.uk")]
        public void UrlRegex_AllowedFormats(string input)
        {
            Regex.Match(input, Constants.RegularExpressions.UrlRegex).Success.Should().BeTrue();
        }

        [TestCase("goalco")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("www.goal. co")]
        public void UrlRegex_InvalidFormats(string input)
        {
            Regex.Match(input, Constants.RegularExpressions.UrlRegex).Success.Should().BeFalse();
        }
    }
}
