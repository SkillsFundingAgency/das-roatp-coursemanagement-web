using System.Text.Json.Serialization;

namespace SFA.DAS.Roatp.CourseManagement.Domain.ApiModels
{
    public class ProviderAccountResponse
    {
        [JsonPropertyName("canAccessService")]
        public bool CanAccessService { get; set; }
    }
}
