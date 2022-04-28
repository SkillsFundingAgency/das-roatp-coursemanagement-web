using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SFA.DAS.Roatp.CourseManagement.Web.Infrastructure.ApiClients
{
    /// <summary>
    /// Base class containing common functionality that all ApiClients should use.
    /// Includes functionality to write an error log entry for any unsuccessful API calls.
    /// Please read documentation on all methods.
    /// </summary>
    /// <typeparam name="AC">The inherited ApiClient.</typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class ApiClientBase<AC>
    {
        protected const string _acceptHeaderName = "Accept";
        protected const string _contentType = "application/json";

        protected readonly HttpClient _httpClient;
        protected readonly ILogger<AC> _logger;

        protected ApiClientBase(HttpClient httpClient, ILogger<AC> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains(_acceptHeaderName))
            {
                _httpClient.DefaultRequestHeaders.Add(_acceptHeaderName, _contentType);
            }
        }

        /// <summary>
        /// HTTP GET to the specified URI
        /// </summary>
        /// <typeparam name="T">The type of the object to read.</typeparam>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (of type T).</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<T> Get<T>(string uri)
        {
            try
            {
                using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
                {

                    await LogErrorIfUnsuccessfulResponse(response);
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<T>();
                    }
                    return default;

                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP POST to the specified URI
        /// </summary>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>The HttpStatusCode, which is the responsibility of the caller to handle.</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<HttpStatusCode> Post<T>(string uri, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, Encoding.UTF8, _contentType)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Post} - {uri}");
                throw;
            }
        }

        private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
        {
            if (response?.RequestMessage != null && !response.IsSuccessStatusCode)
            {
                var callingMethod = new System.Diagnostics.StackFrame(1).GetMethod().Name;

                var httpMethod = response.RequestMessage.Method.ToString();
                var statusCode = (int)response.StatusCode;
                var reasonPhrase = response.ReasonPhrase;
                var requestUri = response.RequestMessage.RequestUri;

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiErrorMessage = responseContent;

                _logger.LogError($"Method: {callingMethod} || HTTP {statusCode} {reasonPhrase} || {httpMethod}: {requestUri} || Message: {apiErrorMessage}");
            }
        }

    }
}