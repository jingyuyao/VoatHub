using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage;
using Windows.Security.Credentials;

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
    public class VoatApiClient : IDisposable
    {
        private HttpClient httpClient;
        private CredentialManager credentialManager;
        private TokenManager tokenManager;

        public VoatApiClient(string apiKey, string tokenUri)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", apiKey);

            credentialManager = new CredentialManager("voatClient");
            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            if (credentialManager.LoggedIn) setAuthorizationHeader();
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetAsync<T>(Uri uri)
        {
            updateHeader();
            HttpResponseMessage response = await this.httpClient.GetAsync(uri);
            return await DeserializeResponse<T>(response);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.PostAsync(Uri, IHttpContent)"/> to send "application/json"
        /// and return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> PostAsync<T>(Uri uri, IHttpContent content)
        {
            updateHeader();
            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.httpClient.PostAsync(uri, content);
            return await DeserializeResponse<T>(response);
        }

        private void setAuthorizationHeader()
        {
            var accessToken = tokenManager.AccessToken;
            if (accessToken != null)
                httpClient.DefaultRequestHeaders.Authorization = new HttpCredentialsHeaderValue("Bearer", accessToken);
        }

        private void updateHeader()
        {
            if (credentialManager.LoggedIn && tokenManager.Expired) setAuthorizationHeader();
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

        public void Login(string username, string password)
        {
            credentialManager.Login(username, password);
        }

        public void Logout()
        {
            credentialManager.Logout();
        }

        public bool LoggedIn
        {
            get
            {
                return credentialManager.LoggedIn;
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
