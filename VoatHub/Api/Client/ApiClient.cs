using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

using VoatHub.Models.Voat;

namespace VoatHub.Api.Client
{
    /// <summary>
    /// Base class for all clients that interacts with the API.
    /// <para>This class provides throttle, token and credential management for
    /// calls made to the API vendor. The user of this client is responsible to handle 
    /// errors returned by this class as well as checking login status before making calls.</para>
    /// <para>Child classes should provided token retrieval, token refresh, client name
    /// and response deserialization methods.</para>
    /// </summary>
    public abstract class ApiClient : IApiClient
    {
        private HttpClient httpClient;
        protected HttpBaseProtocolFilter httpBaseFilter;
        protected CredentialManager credentialManager;
        protected TokenManager tokenManager;
        protected ThrottleManager throttleManager;

        public ApiClient()
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
            throttleManager = new ThrottleManager(ClientName);
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

        /// <summary>
        /// Wraps <see cref="HttpClient.GetAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(Uri uri)
        {
            await preCall();
            Debug.WriteLine("GET " + uri.AbsoluteUri, "ApiClient");
            HttpResponseMessage response = await this.httpClient.GetAsync(uri);
            return await handleResponse<T>(response);
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
        public async Task<T> PostAsync<T>(Uri uri, IHttpContent content)
        {
            await preCall();
            Debug.WriteLine("POST " + uri.AbsoluteUri, "ApiClient");
            Debug.WriteLine("Content " + content, "ApiClient");
            if (content != null) content.Headers.ContentType = requestContentType;
            HttpResponseMessage response = await httpClient.PostAsync(uri, content);
            return await handleResponse<T>(response);
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
        public async Task<T> PutAsync<T>(Uri uri, IHttpContent content)
        {
            await preCall();
            Debug.WriteLine("PUT " + uri.AbsoluteUri, "ApiClient");
            Debug.WriteLine("Content " + content, "ApiClient");
            if (content != null) content.Headers.ContentType = requestContentType;
            HttpResponseMessage response = await httpClient.PutAsync(uri, content);
            return await handleResponse<T>(response);
        }

        /// <summary>
        /// Wraps <see cref="HttpClient.DeleteAsync(Uri)"/> to return an <see cref="ApiResponse{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<T> DeleteAsync<T>(Uri uri)
        {
            await preCall();
            Debug.WriteLine("DELETE " + uri.AbsoluteUri, "ApiClient");
            HttpResponseMessage response = await httpClient.DeleteAsync(uri);
            return await handleResponse<T>(response);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        /// <summary>
        /// Return the name of this client.
        /// <para>This method should always return the same name for a given client.</para>
        /// </summary>
        public abstract string ClientName { get; }
        /// <summary>
        /// Retrieve oauth token from API vendor using given credentials
        /// </summary>
        /// <returns></returns>
        protected abstract Task<ApiToken> retrieveToken(string username, string password);
        /// <summary>
        /// Get new token from API vendor using stored credentials.
        /// </summary>
        /// <returns></returns>
        protected abstract Task<ApiToken> refreshToken();
        /// <summary>
        /// Deserializes response to user defined type using client specific serializer.
        /// </summary>
        /// <typeparam name="T">Type the response is deserialized to.</typeparam>
        /// <param name="response">Response to deserialize.</param>
        /// <returns>Serialized response.</returns>
        protected abstract Task<T> deserializeResponse<T>(HttpResponseMessage response);

        /// <summary>
        /// The ContentType of request data being sent. Default to null.
        /// <para>Child classes should override this if it need to specify the type of
        /// request content like "application/json".</para>
        /// </summary>
        protected virtual HttpMediaTypeHeaderValue requestContentType
        {
            get
            {
                return null;
            }
        }

        protected void addHeader(string name, string value)
        {
            httpClient.DefaultRequestHeaders.Add(name, value);
        }

        protected void removeHeader(string name)
        {
            httpClient.DefaultRequestHeaders.Remove(name);
        }

        /// <summary>
        /// Bundles functions that need to happen before each call.
        /// </summary>
        /// <returns></returns>
        private async Task preCall()
        {
            //Debug.WriteLine("precCall()", "ApiClient");

            await throttleManager.Wait();
            await updateAccessToken();
            setAuthorizationHeader();
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
        private async Task<T> handleResponse<T>(HttpResponseMessage response)
        {
            throttleManager.MadeCall();

            if (response == null) return default(T);
            try
            {
                // Leave error handling to api client user
                var deserialized =  await deserializeResponse<T>(response);
                return deserialized;
            }
            catch (SerializationException)
            {
                // Server returns ill formed data which we can't serialize
                return default(T);
            }
        }
    }
}
