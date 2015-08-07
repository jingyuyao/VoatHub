using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Data;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace VoatHub.Api
{
    public abstract class ApiClient : IApiClient
    {
        protected HttpClient httpClient;
        protected CredentialManager credentialManager;
        protected TokenManager tokenManager;
        protected ThrottleManager throttleManager;

        public ApiClient()
        {
            httpClient = new HttpClient();
            credentialManager = new CredentialManager(ClientName);
            tokenManager = new TokenManager(this);
            throttleManager = new ThrottleManager();
        }

        public abstract string ClientName { get; }
        public abstract Task<ApiToken> RetrieveToken();

        /// <summary>
        /// Deserializes response to user defined type using client specific serializer.
        /// </summary>
        /// <typeparam name="T">Type the response is deserialized to.</typeparam>
        /// <param name="response">Response to deserialize.</param>
        /// <returns>Serialized response.</returns>
        /// <exception cref="SerializationException"></exception>
        /// <seealso cref="JsonApiClient.deserializeResponse{T}(HttpResponseMessage)"/>
        protected abstract Task<T> deserializeResponse<T>(HttpResponseMessage response);

        /// <summary>
        /// Login in the given user.
        /// </summary>
        /// <remarks>Actually implementation is delegated to <see cref="CredentialManager"/></remarks>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Login(string username, string password)
        {
            credentialManager.Login(username, password);
        }

        /// <summary>
        /// Logout the current user. Also clears token.
        /// </summary>
        /// <remarks>Actual implementation is delegated to <see cref="CredentialManager"/>
        /// and <see cref="TokenManager"/></remarks>
        public void Logout()
        {
            credentialManager.Logout();
            tokenManager.Clear();
        }

        /// <summary>
        /// Check if the user is logged in.
        /// </summary>
        /// <remarks>Actual implementation is delegated to <see cref="CredentialManager"/></remarks>
        public bool LoggedIn
        {
            get
            {
                return credentialManager.LoggedIn;
            }
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(Uri uri)
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
        public async Task<T> PostAsync<T>(Uri uri, IHttpContent content)
        {
            await preCall();

            content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await this.httpClient.PostAsync(uri, content);
            return await handleResponse<T>(response);
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
        private async Task<T> handleResponse<T>(HttpResponseMessage response)
        {
            throttleManager.MadeCall();
            try
            {
                // Leave error handling to api client user
                return await deserializeResponse<T>(response);
            }
            catch (SerializationException)
            {
                // Server returns ill formed data which we can't serialize
                return default(T);
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
