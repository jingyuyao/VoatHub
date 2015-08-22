using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;

namespace VoatHubTests.Api
{
    [TestClass]
    public class VoatApiTests
    {
        private static VoatApi api = new VoatApi(TestSettings.ApiKey, TestSettings.Scheme, TestSettings.Host, TestSettings.ApiPath, TestSettings.TokenUri);
        private static string testSub = "swampfire100";
        private static UserSubmission sampleSubmission = new UserSubmission
        {
            Title = "Test test test test!!!",
            Nsfw = false,
            Anonymous = false,
            Content = "Lets hope this works. " + DateTime.Now.ToString(),
            HasState = true
        };

        private static UserSubmission updatedSubmission = new UserSubmission
        {
            Title = "Test update update update!!!",
            Nsfw = false,
            Anonymous = false,
            Content = "To update or update? " + DateTime.Now.ToString(),
            HasState = true
        };

        private static UserValue sampleComment = new UserValue
        {
            Value = "Generic hateful, ignorant and arrogant comment"
        };

        private static UserValue updatedComment = new UserValue
        {
            Value = "Generic cheerful, insightful and humble comment"
        };

        private static ApiUserPreferences samplePerf = new ApiUserPreferences
        {
            DisableCustomCSS = false,
            EnableAdultContent = true,
            ClickingMode = true,
            PubliclyShowSubscriptions = false,
            Language = "french",
            EnableNightMode = false,
            PubliclyDisplayVotes = false,
        };

        [ClassInitialize]
        public static async Task classInit(TestContext context)
        {
            Assert.IsTrue(await api.Login(TestSettings.Username, TestSettings.Password));
        }

        [ClassCleanup]
        public static void classCleanUp()
        {
            api.Dispose();
        }

        [TestMethod]
        public async Task Submissions()
        {
            assertResponse(await api.GetSubmissionList(testSub));
            var title = assertResponse(await api.GetSubmission(testSub, 1)).Data.Title;
            Assert.AreEqual("Wow, Voat on Azure is a pain in the...", title);
            assertResponse(await api.GetSubmission(1));
            var id = assertResponse(await api.PostSubmission(testSub, sampleSubmission)).Data.ID;
            var updatedTitle = assertResponse(await api.PutSubmission(testSub, id, updatedSubmission)).Data.Title;
            Assert.AreEqual(updatedSubmission.Title, updatedTitle);
            var id2 = assertResponse(await api.PostSubmission(testSub, sampleSubmission)).Data.ID;
            var updatedTitle2 = assertResponse(await api.PutSubmission(testSub, id2, updatedSubmission)).Data.Title;
            Assert.AreEqual(updatedSubmission.Title, updatedTitle2);
            assertResponse(await api.DeleteSubmission(testSub, id));
            assertResponse(await api.DeleteSubmission(testSub, id2));
        }

        [TestMethod]
        public async Task Subverse()
        {
            assertResponse(await api.GetSubverseInfo(testSub));
            // Not implemented
            //assertResponse(await api.PostSubverseBlock(testSub));
            //assertResponse(await api.DeleteSubverseBlock(testSub));
        }

        [TestMethod]
        public async Task Comments()
        {
            var submissionId = assertResponse(await api.PostSubmission(testSub, sampleSubmission)).Data.ID;
            var commentId = assertResponse(await api.PostComment(testSub, submissionId, sampleComment)).Data.ID;
            var commentId2 = assertResponse(await api.PostCommentReply(testSub, submissionId, commentId, sampleComment)).Data.ID;
            assertResponse(await api.PostCommentReply(commentId, sampleComment));
            var numComments = assertResponse(await api.GetComment(commentId)).Data.ChildCount;
            //Assert.AreEqual(2, numComments);  // Bugged API
            var updatedContent = assertResponse(await api.PutComment(commentId2, updatedComment)).Data.Content;
            Assert.AreEqual(updatedComment.Value, updatedContent);
            assertResponse(await api.GetCommentList(testSub, submissionId));
            assertResponse(await api.GetCommentList(testSub, submissionId, commentId));
            assertResponse(await api.DeleteComment(commentId));
        }

        [TestMethod]
        public async Task User()
        {
            // Internal server error?!
            //var response = await api.PutPreferences(samplePerf);
            //Assert.IsTrue(response.success);
            assertResponse(await api.GetPreferences());
            assertResponse(await api.UserInfo(TestSettings.Username));
            assertResponse(await api.UserComments(TestSettings.Username));
            assertResponse(await api.UserSubmissions(TestSettings.Username));
            assertResponse(await api.UserSubscriptions(TestSettings.Username));
        }

        [TestMethod]
        public async Task Vote()
        {
            var submissionId = assertResponse(await api.PostSubmission(testSub, sampleSubmission)).Data.ID;
            var commentId = assertResponse(await api.PostComment(testSub, submissionId, sampleComment)).Data.ID;
            assertResponse(await api.PostVote("submission", submissionId, 1));
            assertResponse(await api.PostVote("comment", commentId, 1));
            assertResponse(await api.PostVoteRevokeOnRevote("submission", submissionId, 1, true));
            assertResponse(await api.PostVoteRevokeOnRevote("comment", commentId, 1, true));
        }

        [TestMethod]
        public async Task Save()
        {
            var submissionId = assertResponse(await api.PostSubmission(testSub, sampleSubmission)).Data.ID;
            var commentId = assertResponse(await api.PostComment(testSub, submissionId, sampleComment)).Data.ID;
            assertResponse(await api.PostSubmissionsSave(submissionId));
            assertResponse(await api.DeleteSubmissionsSave(submissionId));
            assertResponse(await api.PostCommentsSave(commentId));
            assertResponse(await api.DeleteCommentsSave(commentId));
        }

        private ApiResponse assertResponse(ApiResponse response)
        {
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            return response;
        }

        private ApiResponse<T> assertResponse<T>(ApiResponse<T> response)
        {
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.Data);
            return response;
        }
    }
}
