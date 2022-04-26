using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.Tokens
{
    public static class BearerTokenGenerator
    {
        private const string JwtBearerScheme = "Bearer";

        public static async Task<AuthenticationHeaderValue> GenerateTokenAsync(string identifier)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(identifier);

            return new AuthenticationHeaderValue(JwtBearerScheme, accessToken);
        }
    }
}
