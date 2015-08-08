using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private static readonly string SUBVERSE_PREFIX = "v/";
        private static readonly string SUBMISSIONS_PREFIX = "submissions/";
        private static readonly string COMMENTS_PREFIX = "comments/";
        private static readonly string USER_PREFIX = "u/";

        private IApiClient apiClient;
        private Uri baseUri;
        private UriBuilder uriBuilder;

        public VoatApi(string apiKey, string baseUri, string tokenUri)
        {
            apiClient = new VoatApiClient(apiKey, tokenUri);
            this.baseUri = new Uri(baseUri);
            uriBuilder = new UriBuilder(baseUri);
        }

        public async Task<bool> Login(string username, string password)
        {
            return await apiClient.Login(username, password);
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

        /// <summary>
        /// Get the list of submissions for a subverse.
        /// </summary>
        /// <param name="subverse">Name of a subverse.</param>
        /// <returns>Task containing the response.</returns>
        public async Task<ApiResponse<List<ApiSubmission>>> GetSubmissionList(string subverse, SearchOptions searchOptions)
        {
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse + Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubmission>>>(uri);
        }

        /// <summary>
        /// Post a <see cref="UserSubmission"/> to a subverse. Required authentication.
        /// </summary>
        /// <param name="subverse"></param>
        /// <param name="submission"></param>
        /// <returns></returns>
        /// <exception cref="UnauthenticatedException">If not authenticated.</exception>
        public async Task<ApiResponse<ApiSubmission>> PostSubmission(string subverse, UserSubmission submission)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse);
            return await apiClient.PostAsync<ApiResponse<ApiSubmission>>(uri, serializeUserSubmission(submission));
        }

        /// <summary>
        /// Get a submission from a subverse.
        /// </summary>
        /// <param name="subverse"></param>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ApiSubmission>> GetSubmission(string subverse, int submissionId)
        {
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse + "/" + submissionId);
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> GetSubmission(int submissionId)
        {
            Uri uri = GetAbsoluteUri(SUBMISSIONS_PREFIX + submissionId);
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(string subverse, int submissionId, UserSubmission submission)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse + "/" + submissionId);
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, serializeUserSubmission(submission));
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(int submissionId, UserSubmission submission)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri(SUBMISSIONS_PREFIX + submissionId);
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, serializeUserSubmission(submission));
        }

        public async Task<ApiResponse> DeleteSubmission(string subverse, int submissionId)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse + "/" + submissionId);
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse> DeleteSubmission(int submissionId)
        {
            requireLogin();
            Uri uri = GetAbsoluteUri(SUBMISSIONS_PREFIX + submissionId);
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse<ApiSubverseInfo>> GetSubverseInfo(string subverse)
        {
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse);
            return await apiClient.GetAsync<ApiResponse<ApiSubverseInfo>>(uri);
        }

        public async Task<ApiResponse> PostSubverseBlock(string subverse)
        {
            Uri uri = GetAbsoluteUri(SUBMISSIONS_PREFIX + subverse);
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteSubverseBlock(string subverse)
        {
            Uri uri = GetAbsoluteUri(SUBMISSIONS_PREFIX + subverse);
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse<List<ApiComment>>> GetCommentList(string subverse, int submissionId, SearchOptions searchOptions)
        {
            Uri uri = GetAbsoluteUri(SUBVERSE_PREFIX + subverse + "/" + submissionId + "/comments" + Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiComment>>>(uri);
        }

        internal void requireLogin()
        {
            if (!apiClient.LoggedIn)
            {
                throw new UnauthenticatedException();
            }
        }

        private HttpStringContent serializeUserSubmission(UserSubmission submission)
        {
            return new HttpStringContent(JsonConvert.SerializeObject(submission));
        }

        /// <summary>
        /// Get the absolute API uri by combining baseUri and a relative uri string.
        /// </summary>
        /// <param name="path">A relative uri. Relative uri should not be prefixed with a '/'.</param>
        /// <returns>The absolute uri of the API endpoint.</returns>
        private Uri GetAbsoluteUri(string path)
        {
            return new Uri(this.baseUri, path);
        }

        public void Dispose()
        {
            apiClient.Dispose();
        }
    }
}
