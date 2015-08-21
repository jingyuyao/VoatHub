using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

using VoatHub.Api.Client;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.Voat;

namespace VoatHub.Api.Voat
{
    /// <summary>
    /// Class to interface to Voat's API
    /// </summary>
    public class VoatApi : IDisposable
    {
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
        private ApiUriBuilder uriBuilder;

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
            uriBuilder = new ApiUriBuilder(scheme, host, apiPath);
        }

        public string UserName
        {
            get
            {
                return apiClient.UserName;
            }
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

        #region Submissions

        public async Task<ApiResponse<List<ApiSubmission>>> GetSubmissionList(string subverse, SearchOptions searchOptions)
        {
            Uri uri = uriBuilder.Uri(String.Format(SUBVERSE, subverse), Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubmission>>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> GetSubmission(string subverse, int submissionId)
        {
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION, subverse, submissionId));
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> GetSubmission(int submissionId)
        {
            Uri uri = uriBuilder.Uri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.GetAsync<ApiResponse<ApiSubmission>>(uri);
        }

        public async Task<ApiResponse<ApiSubmission>> PostSubmission(string subverse, UserSubmission submission)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(String.Format(SUBVERSE, subverse));
            return await apiClient.PostAsync<ApiResponse<ApiSubmission>>(uri, Utility.serializeJson(submission));
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(string subverse, int submissionId, UserSubmission submission)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION, subverse, submissionId));
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, Utility.serializeJson(submission));
        }

        public async Task<ApiResponse<ApiSubmission>> PutSubmission(int submissionId, UserSubmission submission)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.PutAsync<ApiResponse<ApiSubmission>>(uri, Utility.serializeJson(submission));
        }

        public async Task<ApiResponse> DeleteSubmission(string subverse, int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION, subverse, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse> DeleteSubmission(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBMISSIONS, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        #endregion Submissions

        #region Subverse

        public async Task<ApiResponse<ApiSubverseInfo>> GetSubverseInfo(string subverse)
        {
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_INFO, subverse));
            return await apiClient.GetAsync<ApiResponse<ApiSubverseInfo>>(uri);
        }

        // Not implemented
        //public async Task<ApiResponse> PostSubverseBlock(string subverse)
        //{
        //    apiClient.EnsureLoggedIn();
        //    Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_BLOCK, subverse));
        //    return await apiClient.PostAsync<ApiResponse>(uri, null);
        //}

        // Not implemented
        //public async Task<ApiResponse> DeleteSubverseBlock(string subverse)
        //{
        //    apiClient.EnsureLoggedIn();
        //    Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_BLOCK, subverse));
        //    return await apiClient.DeleteAsync<ApiResponse>(uri);
        //}

        #endregion Subverse

        #region Comments

        public async Task<ApiResponse<List<ApiComment>>> GetCommentList(string subverse, int submissionId, SearchOptions searchOptions)
        {
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION_COMMENTS, subverse, submissionId), Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiComment>>>(uri);
        }

        public async Task<ApiResponse<List<ApiComment>>> GetCommentList(string subverse, int submissionId, int parentId, SearchOptions searchOptions)
        {
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION_COMMENTS, subverse, submissionId, parentId), Utility.ToQueryString(searchOptions));
            return await apiClient.GetAsync<ApiResponse<List<ApiComment>>>(uri);
        }

        public async Task<ApiResponse<ApiComment>> GetComment(int commentid)
        {
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS, commentid));
            return await apiClient.GetAsync<ApiResponse<ApiComment>>(uri);
        }

        public async Task<ApiResponse<ApiComment>> PostComment(string subverse, int submissionId, UserValue comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION_REPLY, subverse, submissionId));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, Utility.serializeJson(comment));
        }

        /// <summary>
        /// User this method for inbox comment replies.
        /// </summary>
        /// <param name="commentid"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ApiComment>> PostCommentReply(int commentid, UserValue comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS, commentid));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, Utility.serializeJson(comment));
        }

        /// <summary>
        /// Use this for normal comment replies.
        /// </summary>
        /// <param name="subverse"></param>
        /// <param name="submissionId"></param>
        /// <param name="commentid"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ApiComment>> PostCommentReply(string subverse, int submissionId, int commentid, UserValue comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBVERSE_SUBMISSION_COMMENT_REPLY, subverse, submissionId, commentid));
            return await apiClient.PostAsync<ApiResponse<ApiComment>>(uri, Utility.serializeJson(comment));
        }

        public async Task<ApiResponse<ApiComment>> PutComment(int commentid, UserValue comment)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS, commentid));
            return await apiClient.PutAsync<ApiResponse<ApiComment>>(uri, Utility.serializeJson(comment));
        }

        public async Task<ApiResponse> DeleteComment(int commentid)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS, commentid));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        #endregion Comments

        #region User

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Sucess flag is false if preferences has never been set.</returns>
        public async Task<ApiResponse<ApiUserPreferences>> GetPreferences()
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(USER_PREFERENCES);
            return await apiClient.GetAsync<ApiResponse<ApiUserPreferences>>(uri);
        }

        // Internal server error
        //public async Task<ApiResponse> PutPreferences(ApiUserPreferences preferences)
        //{
        //    apiClient.EnsureLoggedIn();
        //    Uri uri = uriBuilder.Uri(USER_PREFERENCES);
        //    return await apiClient.PutAsync<ApiResponse>(uri, Utility.serializeJson(preferences));
        //}

        public async Task<ApiResponse<ApiUserInfo>> UserInfo(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(USER_INFO, username));
            return await apiClient.GetAsync<ApiResponse<ApiUserInfo>>(uri);
        }

        public async Task<ApiResponse<List<ApiUserInfo>>> UserComments(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(USER_COMMENTS, username));
            return await apiClient.GetAsync<ApiResponse<List<ApiUserInfo>>>(uri);
        }

        public async Task<ApiResponse<List<ApiSubmission>>> UserSubmissions(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(USER_SUBMISSIONS, username));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubmission>>>(uri);
        }

        public async Task<ApiResponse<List<ApiSubscription>>> UserSubscriptions(string username)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(USER_SUBSCRIPTIONS, username));
            return await apiClient.GetAsync<ApiResponse<List<ApiSubscription>>>(uri);
        }

        // Not yet implemented by the Voat API
        //public async Task<ApiResponse<UserInfo>> UserSaved()
        //{
        //    apiClient.EnsureLoggedIn();
        //    Uri uri = uriBuilder.Uri(USER_SAVED);
        //    return await apiClient.GetAsync<ApiResponse<UserInfo>>(uri);
        //}

        #endregion User

        #region Voting

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">One of 'submission', 'comment'</param>
        /// <param name="id"></param>
        /// <param name="vote"></param>
        /// <returns></returns>
        public async Task<ApiResponse<VoteResponse>> PostVote(string type, int id, int vote)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(VOTE, type, id, vote));
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
            Uri uri = uriBuilder.Uri(string.Format(VOTE_ON_REVOKE, type, id, vote, revokeOnRevote));
            return await apiClient.PostAsync<ApiResponse<VoteResponse>>(uri, null);
        }

        #endregion Voting

        #region Saving

        public async Task<ApiResponse> PostSubmissionsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBMISSIONS_SAVE, submissionId));
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteSubmissionsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(SUBMISSIONS_SAVE, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        public async Task<ApiResponse> PostCommentsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS_SAVE, submissionId));
            return await apiClient.PostAsync<ApiResponse>(uri, null);
        }

        public async Task<ApiResponse> DeleteCommentsSave(int submissionId)
        {
            apiClient.EnsureLoggedIn();
            Uri uri = uriBuilder.Uri(string.Format(COMMENTS_SAVE, submissionId));
            return await apiClient.DeleteAsync<ApiResponse>(uri);
        }

        #endregion Saving

        public void Dispose()
        {
            apiClient.Dispose();
        }
    }
}
