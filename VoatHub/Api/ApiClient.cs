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
    /// <summary>An HttpClient tailored for Voat's API.
    /// <para>
    /// Adds Voat-ApiKey on every request and deserializes Json response into
    /// the approperiate class.
    /// </para>
    /// </summary>
    public class ApiClient : IDisposable
    {
        private HttpClient httpClient;

        public ApiClient(string apiKey)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", apiKey);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetAsync<T>(Uri uri)
        {
            HttpResponseMessage response = await this.httpClient.GetAsync(uri);
            return await DeserializeResponse<T>(response);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.PostAsync(Uri, IHttpContent)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> PostAsync<T>(Uri uri, IHttpContent content)
        {
            HttpResponseMessage response = await this.httpClient.PostAsync(uri, content);
            return await DeserializeResponse<T>(response);
        }

        /// <summary>
        /// Deserializes response containing JSON data into an ApiResponse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<ApiResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse<T>>(responseContent);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
