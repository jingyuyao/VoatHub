using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.Web.Http;

using Newtonsoft.Json;

using VoatHub.Api.Client;
using VoatHub.Data.Voat;

namespace VoatHub.Api.Voat
{
    /// <summary>
    /// Class to interface to Voat's API
    /// </summary>
    public class VoatApi : IDisposable
    {
        //private static readonly string LEVEL1 = "{0}";
        //private static readonly string LEVEL2 = "{0}/{1}";
        //private static readonly string LEVEL3 = "{0}/{1}/{2}";
        //private static readonly string SUBVERSE_PREFIX = "v/";
        //private static readonly string SUBMISSIONS_PREFIX = "submissions/";
        //private static readonly string COMMENTS_PREFIX = "comments/";
        //private static readonly string USER_PREFIX = "u/";

        private static readonly string SUBVERSE = "v/{0}";
        private static readonly string SUBVERSE_SUBMISSION = SUBVERSE + "/{1}";
        private static readonly string SUBVERSE_SUBMISSION_COMMENTS = SUBVERSE_SUBMISSION + "/comments";
        private static readonly string SUBVERSE_INFO = SUBVERSE + "/info";
        private static readonly string SUBVERSE_SUBMISSION_REPLY = SUBVERSE_SUBMISSION + "/comment";
        private static readonly string SUBVERSE_SUBMISSION_COMMENT_REPLY = SUBVERSE_SUBMISSION_REPLY + "/{2}";
        private static readonly string SUBVERSE_BLOCK = SUBVERSE + "/block";

        private static readonly string SUBMISSIONS = "submissions/{0}";
        private static readonly string SUBMISSIONS_SAVE = SUBMISSIONS + "/save";

        private static readonly string COMMENTS = "comments/{0}";
        private static readonly string COMMENTS_SAVE = COMMENTS + "/save";

        private static readonly string USER_PREFERENCES = "u/preferences";
        private static readonly string USER_SAVED = "u/saved";
        private static readonly string USER = "u/{0}";
        private static readonly string USER_INFO = USER + "/info";
        private static readonly string USER_COMMENTS = USER + "/comments";
        private static readonly string USER_SUBMISSIONS = USER + "/submissions";
        private static readonly string USER_SUBSCRIPTIONS = USER + "/subscriptions";

        private static readonly string USER_MESSAGE = "u/messages";
        private static readonly string USER_MESSAGE_REPLY = USER_MESSAGE + "/reply/{0}";
        private static readonly string USER_MESSAGE_GET = USER_MESSAGE + "/{0}/{1}";

        private static readonly string VOTE = "vote/{0}/{1}/{2}";
        private static readonly string VOTE_ON_REVOKE = VOTE + "?revokeOnRevote={3}";

        private IApiClient apiClient;
        private UriBuilder uriBuilder;
        private string apiPath;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="host">Full domain path with ending slash.</param>
        /// <param name="apiPath">Relative path to top level API endpoint with ending slash.</param>
        /// <param name="tokenUri">Full path to tokenUri with or without ending slash.</param>
        public VoatApi(string apiKey, string scheme, string host, string apiPath, string tokenUri)
        {
            apiClient = new VoatApiClient(apiKey, tokenUri);
            uriBuilder = new UriBuilder();
            uriBuilder.Scheme = scheme;
            uriBuilder.Host = host;
            this.apiPath = apiPath;
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
            Uri uri = GetAbsoluteUri(String.Format(SUBVERSE, subverse), Utility.ToQueryString(searchOptions));
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
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(String.Format(SUBVERSE, subverse));
            return await apiClient.PostAsync<ApiResponse<ApiSubmission>>(uri, serializeToContent(submission));
        }

        /// <summary>
        /// Get a submission from a subverse.
        /// </summary>
        /// <param name="subverse"></param>
        /// <param name="submissionId"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ApiSubmission>> GetSubmission(string subverse, int submissionId)
        {
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION, subverse,submissionId));
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> GetSubmission(int submissionId)
        {
            Uri uri = GetAbsoluteUri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(string subverse, int submissionId, UserSubmission submission)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION, subverse, submissionId));
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, serializeToContent(submission));
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(int submissionId, UserSubmission submission)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, serializeToContent(submission));
        }

        public async Task<ApiResponse> DeleteSubmission(string subverse, int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION, subverse, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse> DeleteSubmission(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse<ApiSubverseInfo>> GetSubverseInfo(string subverse)
        {
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_INFO, subverse));
            return await apiClient.GetAsync<ApiResponse<ApiSubverseInfo>>(uri);
        }

        public async Task<ApiResponse> PostSubverseBlock(string subverse)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_BLOCK, subverse));
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteSubverseBlock(string subverse)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_BLOCK, subverse));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse<List<ApiComment>>> GetCommentList(string subverse, int submissionId, SearchOptions searchOptions)
        {
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION_COMMENTS, subverse, submissionId), Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiComment>>>(uri);
        }

        public async Task<ApiResponse<List<ApiComment>>> GetCommentListFromParent(string subverse, int submissionId, int parentId, SearchOptions searchOptions)
        {
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION_COMMENTS, subverse, submissionId, parentId), Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiComment>>>(uri);
        }

        public async Task<ApiResponse<ApiComment>> GetComment(int commentid)
        {
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS, commentid));
            return await apiClient.GetAsync<ApiResponse<ApiComment>>(uri);
        }

        public async Task<ApiResponse<ApiComment>> PostComment(int commentid, UserComment comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS, commentid));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, serializeToContent(comment));
        }

        public async Task<ApiResponse<ApiComment>> PutComment(int commentid, UserComment comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS, commentid));
            return await apiClient.PutAsync<ApiResponse<ApiComment>>(uri, serializeToContent(comment));
        }

        public async Task<ApiResponse> DeleteComment(int commentid)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS, commentid));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse<ApiComment>> PostComment(string subverse, int submissionId, int commentid, UserComment comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION_COMMENT_REPLY, subverse, submissionId, commentid));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, serializeToContent(comment));
        }

        public async Task<ApiResponse<ApiComment>> PostComment(string subverse, int submissionId, UserComment comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBVERSE_SUBMISSION_REPLY, subverse, submissionId));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, serializeToContent(comment));
        }

        public async Task<ApiResponse<ApiUserPreferences>> GetPreferences()
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(USER_PREFERENCES);
            return await apiClient.GetAsync<ApiResponse<ApiUserPreferences>>(uri);
        }

        public async Task<ApiResponse> PutPreferences(ApiUserPreferences preferences)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(USER_PREFERENCES);
            return await apiClient.PutAsync<ApiResponse>(uri, serializeToContent(preferences));
        }

        public async Task<ApiResponse<UserInfo>> UserInfo(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(USER_INFO, username));
            return await apiClient.GetAsync<ApiResponse<UserInfo>>(uri);
        }

        public async Task<ApiResponse<List<UserInfo>>> UserComments(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(USER_COMMENTS, username));
            return await apiClient.GetAsync<ApiResponse<List<UserInfo>>>(uri);
        }

        public async Task<ApiResponse<List<ApiSubmission>>> UserSubmissions(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(USER_SUBMISSIONS, username));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubmission>>>(uri);
        }

        public async Task<ApiResponse<List<ApiSubscription>>> UserSubscriptions(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(USER_SUBSCRIPTIONS, username));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubscription>>>(uri);
        }

        // Not implemented by Api
        //public async Task<ApiResponse<UserInfo>> UserSaved()
        //{
        //    apiClient.EnsureLoggedIn();
        //    Uri uri = GetAbsoluteUri(USER_SAVED);
        //    return await apiClient.GetAsync<ApiResponse<UserInfo>>(uri);
        //}

        public async Task<ApiResponse<VoteResponse>> PostVote(string type, int id, int vote)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(VOTE, type, id, vote));
            return await apiClient.PostAsync<ApiResponse<VoteResponse>>(uri, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="vote"></param>
        /// <param name="revokeOnRevote">
        /// Optional. If true then a duplicate vote will revoke (undo) the existing vote, 
        /// if false then a duplicate vote will be ignored. 
        /// Choosing to use this setting depends on your UI and how your users will interact with it. 
        /// If a user upvotes a submission but then wants to remove the upvote they typically upvote 
        /// the submission a second time, thus revoking the original upvote and now the submission will 
        /// be in an unvoted/revoked state for the user. Default value is [true].</param>
        /// <returns></returns>
        public async Task<ApiResponse<VoteResponse>> PostVoteRevokeOnRevote(string type, int id, int vote, bool revokeOnRevote)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(VOTE_ON_REVOKE, type, id, vote, revokeOnRevote));
            return await apiClient.PostAsync<ApiResponse<VoteResponse>>(uri, null);
        }

        public async Task<ApiResponse> PostSubmissionsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBMISSIONS_SAVE, submissionId));
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteSubmissionsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(SUBMISSIONS_SAVE, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse> PostCommentsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS_SAVE, submissionId));
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteCommentsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = GetAbsoluteUri(string.Format(COMMENTS_SAVE, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        private HttpStringContent serializeToContent(object anySerializeable)
        {
            return new HttpStringContent(JsonConvert.SerializeObject(anySerializeable));
        }

        /// <summary>
        /// Get the absolute API uri by combining domain, apiPath and a relative uri string.
        /// </summary>
        /// <param name="path">A relative uri. Relative uri should not be prefixed with a '/'.</param>
        /// <returns>The absolute uri of the API endpoint.</returns>
        private Uri GetAbsoluteUri(string path, string query)
        {
            uriBuilder.Path = apiPath + path;
            uriBuilder.Query = query;

            return uriBuilder.Uri;
        }

        private Uri GetAbsoluteUri(string path)
        {
            return GetAbsoluteUri(path, null);
        }

        public void Dispose()
        {
            apiClient.Dispose();
        }
    }
}
