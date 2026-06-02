using System.Linq;
using System.Net;
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

        // Check DNS lookup first
        try
        {
            var hostEntry = await Dns.GetHostEntryAsync(domain);
            if (hostEntry.AddressList.Length > 0)
            {
                return true;
            }

        }
        catch
        {
            // Ignore DNS lookup failures and proceed to check MX records
        }

        // If DNS lookup fails, check for MX records
        var lookup = new LookupClient();

        var results = await lookup.QueryAsync(domain, QueryType.MX);

        return results.Answers.Any(x => x.RecordType == ResourceRecordType.MX);
    }
}
