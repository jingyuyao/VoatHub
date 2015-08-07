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
        private static readonly TimeSpan THROTTLE_WAIT_TIME = new TimeSpan(0, 0, 1);
        private HttpClient httpClient;
        private CredentialManager credentialManager;
        private TokenManager tokenManager;
        private ThrottleManager throttleManager;

        public VoatApiClient(string apiKey, string tokenUri)
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", apiKey);

            credentialManager = new CredentialManager("voatClient");
            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            throttleManager = new ThrottleManager();
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetAsync<T>(Uri uri)
        {
            await preCall();

            HttpResponseMessage response = await this.httpClient.GetAsync(uri);
            return await handleResponse<T>(response);
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
            await preCall();

            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.httpClient.PostAsync(uri, content);
            return await handleResponse<T>(response);
        }

        public void Login(string username, string password)
        {
            credentialManager.Login(username, password);
        }

        public void Logout()
        {
            credentialManager.Logout();
            tokenManager.Clear();
        }

        public bool LoggedIn
        {
            get
            {
                return credentialManager.LoggedIn;
            }
        }
        
        /// <summary>
        /// Bundles functions that need to happen before each call.
        /// </summary>
        /// <returns></returns>
        private async Task preCall()
        {
            await Task.Delay(throttleManager.WaitTime);
            await setAuthorizationHeader();
        }

        /// <summary>
        /// Manages the authorization header.
        /// <para>Set Bearer token if there is a valid one.</para>
        /// </summary>
        /// <returns></returns>
        private async Task setAuthorizationHeader()
        {
            var accessToken = await tokenManager.AccessToken();
            var headers = httpClient.DefaultRequestHeaders;
            if (accessToken == null)
                headers.Authorization = null;
            else if (headers.Authorization == null || headers.Authorization.Token != accessToken)
                headers.Authorization = new HttpCredentialsHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// Safely deserializes the response and notify throttleManager a call has been made.
        /// <para>Throws <see cref="ThrottleException"/> if the request is being throttled.</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns>The API response or null if serialization fails.</returns>
        private async Task<ApiResponse<T>> handleResponse<T>(HttpResponseMessage response)
        {
            throttleManager.MadeCall();
            try
            {
                // Leave error handling to api client user
                return await deserializeResponse<T>(response);
            }
            catch (JsonSerializationException)
            {
                // Server returns ill formed data which we can't serialize
                return null;
            }
        }

        /// <summary>
        /// Deserializes response containing JSON data into an ApiResponse.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<ApiResponse<T>> deserializeResponse<T>(HttpResponseMessage response)
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
