using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using Newtonsoft.Json;

using VoatHub.Data;

namespace VoatHub.Api
{
    public class ApiClient
    {
        private HttpClient httpClient;
        private Uri baseUri;

        public ApiClient(string apiKey, string baseUri)
        {
            httpClient = new HttpClient();
            this.baseUri = new Uri(baseUri);

            var headers = httpClient.DefaultRequestHeaders;

            headers.Add("Voat-ApiKey", apiKey);
        }

        /// <summary>
        /// Get the list of submissions for a subverse.
        /// </summary>
        /// <param name="subverse">Name of a subverse.</param>
        /// <returns>Task containing the response.</returns>
        public async Task<ApiResponse<List<ApiSubmission>>> GetSubmissions(string subverse)
        {
            return await GetAndDeserialize<List<ApiSubmission>>("v/" + subverse);
        }

        /// <summary>
        /// Returns deserialized ApiResponse from a get request to the absolute uri formed using the given relativeUri.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        private async Task<ApiResponse<T>> GetAndDeserialize<T>(string relativeUri)
        {
            Uri uri = GetAbsoluteUri(relativeUri);
            HttpResponseMessage response = await this.httpClient.GetAsync(uri);
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
        }

        /// <summary>
        /// Get the absolute API uri by combining baseUri and a relative uri string.
        /// </summary>
        /// <param name="relativeUri">A relative uri. Relative uri should not be prefixed with a '/'.</param>
        /// <returns>The absolute uri of the API endpoint.</returns>
        private Uri GetAbsoluteUri(string relativeUri)
        {
            return new Uri(this.baseUri, relativeUri);
        }
    }
}
