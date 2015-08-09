using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Web.Http;

using Newtonsoft.Json;

using VoatHub.Api.Voat;
using VoatHub.Data.Voat;

namespace VoatHubTests.Api.Voat
{
    [TestClass]
    public class VoatApiClientTests
    {
        private Uri submissionListUri = new Uri(TestSettings.FullApiPath + "v/Test");
        private UserSubmission submission;
        private HttpStringContent serializedSubmission;
        private VoatApiClient apiClient = new VoatApiClient(TestSettings.ApiKey, TestSettings.TokenUri);

        [TestInitialize]
        public void setUp()
        {
            submission = new UserSubmission();
            submission.title = "Test test test test!!!";
            submission.nsfw = false;
            submission.anon = false;
            submission.content = "Lets hope this works. " + DateTime.Now.ToString();
            submission.HasState = true;

            serializedSubmission = new HttpStringContent(JsonConvert.SerializeObject(submission));
        }

        [TestCleanup]
        public void cleanUp()
        {
            apiClient.Logout();
            apiClient.Dispose();
        }

        [TestMethod]
        public async Task Login()
        {
            bool success = await apiClient.Login(TestSettings.Username, TestSettings.Password);
            Assert.IsTrue(success);
            Assert.IsTrue(apiClient.LoggedIn);
        }

        [TestMethod]
        public async Task Logout()
        {
            await Login();
            apiClient.Logout();
            Assert.IsFalse(apiClient.LoggedIn);
        }

        [TestMethod]
        public async Task GetAsync()
        {
            var data = await apiClient.GetAsync<ApiResponse<List<ApiSubmission>>>(submissionListUri);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public async Task PostAsync()
        {
            var data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsFalse(data.success);

            await Login();

            data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsTrue(data.success);

        }

        [TestMethod]
        public async Task PutAsync()
        {
            await Login();
            var data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsNotNull(data.data);
            int submissionId = data.data.id;

            submission.content = "Update content test";
            serializedSubmission = new HttpStringContent(JsonConvert.SerializeObject(submission));

            var submissionUri = new Uri(TestSettings.FullApiPath + "v/Test/" + submissionId);

            var putData = await apiClient.PutAsync<ApiResponse<ApiSubmission>>(submissionUri, serializedSubmission);
            Assert.IsNotNull(putData.data);
            Assert.AreEqual("Update content test", putData.data.content);
        }

        [TestMethod]
        public async Task DeleteAsync()
        {
            await Login();
            var data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsNotNull(data.data);
            int submissionId = data.data.id;
            var submissionUri = new Uri(TestSettings.FullApiPath + "v/Test/" + submissionId);
            var deleteData = await apiClient.DeleteAsync<ApiResponse>(submissionUri);
            Assert.IsTrue(deleteData.success);
        }
    }
}
