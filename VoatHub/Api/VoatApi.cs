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
    /// <summary>
    /// Class to interface to Voat's API
    /// </summary>
    public class VoatApi : IDisposable
    {
        private VoatApiClient apiClient;
        private Uri baseUri;

        public VoatApi(string apiKey, string baseUri, string tokenUri)
        {
            apiClient = new VoatApiClient(apiKey, tokenUri);
            this.baseUri = new Uri(baseUri);
        }

        /// <summary>
        /// Get the list of submissions for a subverse.
        /// </summary>
        /// <param name="subverse">Name of a subverse.</param>
        /// <returns>Task containing the response.</returns>
        public async Task<ApiResponse<List<ApiSubmission>>> GetSubmissions(string subverse)
        {
            // TODO Search options support
            Uri uri = GetAbsoluteUri("v/" + subverse);
            return await apiClient.GetAsync<List<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> PostSubmission(string subverse, UserSubmission submission)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri("v/" + subverse);

            var content = new HttpStringContent(JsonConvert.SerializeObject(submission));

            return await apiClient.PostAsync<ApiSubmission>(uri, content);
        }

        public void Login(string username, string password)
        {
            apiClient.Login(username, password);
        }

        public void Logout()
        {
            apiClient.Logout();
        }

        public bool LoggedIn
        {
            get
            {
                return apiClient.LoggedIn;
            }
        }

        private void requireLogin()
        {
            if (!apiClient.LoggedIn)
            {
                throw new UnauthenticatedException();
            }
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

        public void Dispose()
        {
            apiClient.Dispose();
        }
    }

    public class UnauthenticatedException : Exception
    { }
}
