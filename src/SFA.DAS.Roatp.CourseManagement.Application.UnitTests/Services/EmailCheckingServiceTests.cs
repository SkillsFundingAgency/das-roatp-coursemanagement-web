using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.CourseManagement.Application.Services;

namespace SFA.DAS.Roatp.CourseManagement.Application.UnitTests.Services;
public class EmailCheckingServiceTests
{
    [TestCase("xxx", false)]
    [TestCase("", false)]
    [TestCase(null, false)]
    [TestCase("aaaa@", false)]
    [TestCase("@aaaa", false)]
    [TestCase("aaaa@NonExistentDomain50c2413d-e8e4-4330-9859-222567ad0f64.co.uk", false)]
    [TestCase("aaaa@google.com", true)]
    [TestCase("google.com", true)]
    public void Email_IsValidDomain_ReturnedExpected(string email, bool isValid)
    {
        var isEmailValid = EmailCheckingService.IsValidDomain(email);
        isEmailValid.Should().Be(isValid);
    }
}
