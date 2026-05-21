using System.Linq;
using System.Threading.Tasks;
using DnsClient;
using DnsClient.Protocol;

namespace SFA.DAS.Roatp.CourseManagement.Application.Services;

public static class EmailCheckingService
{
    public static async Task<bool> IsValidDomain(string email)
    {
        if (email == null)
        {
            return false;
        }

        var domain = email.Contains('@')
            ? email.Split('@')[1]
            : email;

        if (string.IsNullOrEmpty(domain))
        {
            return false;
        }

        var lookup = new LookupClient();

        var results = await lookup.QueryAsync(domain, QueryType.MX);

        return results.Answers.Any(x => x.RecordType == ResourceRecordType.MX);
    }
}
