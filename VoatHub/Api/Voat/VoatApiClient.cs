using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Api.Client;
using VoatHub.Models.Voat;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using System.Threading;
using Newtonsoft.Json;

namespace VoatHub.Api.Voat
{
    /// <summary>An ApiClient tailored for Voat's API.
    /// <para>
    /// Adds Voat-ApiKey on every request and manages token, crendential and throttle.
    /// The user of this client is responsible to handle errors returned by this class
    /// as well as checking login status before making calls.
    /// </para>
    /// </summary>
    public sealed class VoatApiClient
    {
        public static readonly string CLIENT_NAME = "voatClient";
        private static readonly HttpMediaTypeHeaderValue REQUEST_HEADER_VALUE = new HttpMediaTypeHeaderValue("application/json");
        private static readonly int MAX_RETRIES = 3;
        private static readonly TimeSpan RETRY_WAIT_TIME = TimeSpan.FromMilliseconds(1100);

        #region Properties
        private Uri tokenUri;
        private HttpClient httpClient;
        private HttpBaseProtocolFilter httpBaseFilter;
        private CredentialManager credentialManager;
        private TokenManager tokenManager;
        #endregion

        public VoatApiClient(string apiKey, string tokenUri)
        {
            // Prevents windows from attempting to get user credential from the UI.
            // http://stackoverflow.com/questions/24361588/windows-web-http-httpclientgetasync-throws-an-incomplete-exception-when-invalid
            httpBaseFilter = new HttpBaseProtocolFilter
            {
                AllowUI = false,
                AllowAutoRedirect = false
            };
            httpClient = new HttpClient(httpBaseFilter);
            credentialManager = new CredentialManager(ClientName);
            tokenManager = new TokenManager(ClientName);

            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", apiKey);
            this.tokenUri = new Uri(tokenUri);
        }

        public string ClientName
        {
            get
            {
                return CLIENT_NAME;
            }
        }

        #region UserAccount
        public string UserName
        {
            get
            {
                var creds = credentialManager.Credential;
                if (creds != null)
                    return creds.UserName;
                else
                    return null;
            }
        }

        /// <summary>
        /// Login in the given user.
        /// </summary>
        /// <remarks>Actually implementation is delegated to <see cref="CredentialManager"/></remarks>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public async Task<bool> Login(string username, string password)
        {
            // Logic for user is already logged in
            if (credentialManager.LoggedIn)
            {
                var credentail = credentialManager.Credential;
                // Log out old user if its different username
                if (username != credentail.UserName)
                {
                    credentialManager.Logout();
                }
                // Else do nothing and return success
                else
                {
                    return true;
                }
            }

            if (credentialManager.LoggedIn)
            {
                throw new InvalidOperationException("Could not successfully log out previous account");
            }

            // Normal login in process
            var token = await retrieveToken(username, password);

            if (token.access_token != null)
            {
                tokenManager.SetToken(token);
                credentialManager.Login(username, password);
                return true;
            }
            return false;
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

        public void EnsureLoggedIn()
        {
            if (!LoggedIn)
            {
                throw new UnauthenticatedException();
            }
        }
        #endregion

        #region Http
        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> GetAsync<T>(Uri uri)
        {
            return await sendRequest<T>(HttpMethod.Get, uri, null);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.PostAsync(Uri, IHttpContent)"/>.
        /// and return an <see cref="ApiResponse{T}"/>
        /// <para>ContentType is set to <see cref="requestContentType"/></para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> PostAsync<T>(Uri uri, IHttpContent content)
        {
            return await sendRequest<T>(HttpMethod.Post, uri, content);
        }

        public async Task<ApiResponse> PostAsync(Uri uri, IHttpContent content)
        {
            return await sendRequest(HttpMethod.Post, uri, content);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.PutAsync(Uri, IHttpContent)"/>.
        /// and return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <para>ContentType is set to <see cref="requestContentType"/></para>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> PutAsync<T>(Uri uri, IHttpContent content)
        {
            return await sendRequest<T>(HttpMethod.Put, uri, content);
        }

        public async Task<ApiResponse> PutAsync(Uri uri, IHttpContent content)
        {
            return await sendRequest(HttpMethod.Put, uri, content);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.DeleteAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse<T>> DeleteAsync<T>(Uri uri)
        {
            return await sendRequest<T>(HttpMethod.Delete, uri, null);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.DeleteAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<ApiResponse> DeleteAsync(Uri uri)
        {
            return await sendRequest(HttpMethod.Post, uri, null);
        }
        #endregion

        #region Private
        private async Task<HttpRequestMessage> createRequest(HttpMethod method, Uri uri, IHttpContent content)
        {
            await updateAccessToken();
            setAuthorizationHeader();

            var request = new HttpRequestMessage(method, uri);

            if (method != HttpMethod.Get || method != HttpMethod.Delete && content != null)
            {
                content.Headers.ContentType = REQUEST_HEADER_VALUE;
                request.Content = content;
            }

            Debug.WriteLine(string.Format("Method: {0} Uri: {1}", method, uri));
            return request;
        }

        private async Task<ApiResponse<T>> sendRequest<T>(HttpMethod method, Uri uri, IHttpContent content, int retries = 0)
        {
            var request = await createRequest(method, uri, content);
            var response = await httpClient.SendRequestAsync(request);
            var result = await handleResponse<T>(response);
            if (await retryChecker(result as ApiResponse, retries))
            {
                return await sendRequest<T>(method, uri, content, retries + 1);
            }
            return result;
        }

        private async Task<ApiResponse> sendRequest(HttpMethod method, Uri uri, IHttpContent content, int retries = 0)
        {
            var request = await createRequest(method, uri, content);
            var response = await httpClient.SendRequestAsync(request);
            var result = await handleResponse(response);
            if (await retryChecker(result, retries))
            {
                return await sendRequest(method, uri, content, retries + 1);
            }
            return result;
        }

        private async Task<bool> retryChecker(ApiResponse response, int retries)
        {
            if (response != null && !response.Success && response.Error.Type == "ApiThrottleLimit" && retries < MAX_RETRIES)
            {
                Debug.WriteLine("Waiting...");
                await Task.Delay(RETRY_WAIT_TIME);
                return true;
            }
            return false;
        }

        private async Task<ApiToken> retrieveToken(string username, string password)
        {
            Debug.WriteLine("Retrieving token...", "VoatApiClient");

            // Does not need authorization header.
            // Calling setAuthorizationHeader can also cause circular reference

            var properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("grant_type", "password"));
            properties.Add(new KeyValuePair<string, string>("username", username));
            properties.Add(new KeyValuePair<string, string>("password", password));

            var content = new HttpFormUrlEncodedContent(properties);
            var response = await httpClient.PostAsync(tokenUri, content);

            if (!response.IsSuccessStatusCode || response.Content == null)
                return null;

            var token = await deserializeResponse<ApiToken>(response);

            Debug.WriteLine(token, "VoatApiClient");

            return token;
        }

        private async Task<ApiToken> refreshToken()
        {
            var credential = credentialManager.Credential;
            if (credential != null)
            {
                return await retrieveToken(credential.UserName, credential.Password);
            }

            return null;
        }

        /// <summary>
        /// Update access token if user is logged in and token is expired.
        /// </summary>
        /// <returns></returns>
        private async Task updateAccessToken()
        {
            if (credentialManager.LoggedIn && tokenManager.Expired)
            {
                tokenManager.SetToken(await refreshToken());
            }
        }

        /// <summary>
        /// Set authorization header if access token changed.
        /// </summary>
        /// <returns></returns>
        private void setAuthorizationHeader()
        {
            var headers = httpClient.DefaultRequestHeaders;
            string accessToken = tokenManager.AccessToken;

            if (accessToken == null)
                headers.Authorization = null;
            else if (headers.Authorization == null || headers.Authorization.Token != accessToken)
                headers.Authorization = new HttpCredentialsHeaderValue("Bearer", accessToken);
        }

        /// <summary>
        /// Safely deserializes the response and notify throttleManager a call has been made.
        /// <para>Throws <see cref="ThrottleException"/> if the request is being throttled.</para>
        /// </summary>
        /// <remarks>The generic type is not restricted to <see cref="ApiResponse"/> or
        /// similar interface is because sometimes the ApiClient will have to handle ill formed
        /// data such as Oauth tokens. </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns>The API response or default value of the given type if anything fails.</returns>
        /// <exception cref="SerializationException"></exception>
        /// <seealso cref="JsonApiClient.deserializeResponse{T}(HttpResponseMessage)"/>
        private async Task<ApiResponse<T>> handleResponse<T>(HttpResponseMessage response)
        {
            if (response == null) return default(ApiResponse<T>);
            try
            {
                // Leave error handling to api client user
                var deserialized = await deserializeResponse<ApiResponse<T>>(response);
                return deserialized;
            }
            catch (SerializationException)
            {
                // Server returns ill formed data which we can't serialize
                return default(ApiResponse<T>);
            }
        }

        private async Task<ApiResponse> handleResponse(HttpResponseMessage response)
        {
            if (response == null) return default(ApiResponse);
            try
            {
                // Leave error handling to api client user
                var deserialized = await deserializeResponse<ApiResponse>(response);
                return deserialized;
            }
            catch (SerializationException)
            {
                // Server returns ill formed data which we can't serialize
                return default(ApiResponse);
            }
        }

        /// <summary>
        /// Deserializes Json response into user defined type.
        /// </summary>
        /// <typeparam name="T">Type the response is deserialized to.</typeparam>
        /// <param name="response">Response to deserialize.</param>
        /// <returns>Serialized response.</returns>
        /// <exception cref="SerializationException"></exception>
        private async Task<T> deserializeResponse<T>(HttpResponseMessage response)
        {
            try
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent, "VoatApiClient");

                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (JsonException e)
            {
                Debug.WriteLine(e);
                throw new SerializationException();
            }
        }
        #endregion

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}
