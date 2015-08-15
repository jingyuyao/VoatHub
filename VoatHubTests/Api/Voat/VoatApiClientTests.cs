using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Web.Http;

using Newtonsoft.Json;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;

namespace VoatHubTests.Api.Voat
{
    [TestClass]
    public class VoatApiClientTests
    {
        private Uri submissionListUri = new Uri(TestSettings.FullApiPath + "v/swampfire100");
        private UserSubmission submission;
        private HttpStringContent serializedSubmission;
        private VoatApiClient apiClient = new VoatApiClient(TestSettings.ApiKey, TestSettings.TokenUri);

        [TestInitialize]
        public void setUp()
        {
            submission = new UserSubmission();
            submission.Title = "Test test test test!!!";
            submission.Nsfw = false;
            submission.Anonymous = false;
            submission.Content = "Lets hope this works. " + DateTime.Now.ToString();
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
        public async Task BadLogin()
        {
            bool success = await apiClient.Login("sgasdafsdfas", "fasdfasofanf");
            Assert.IsFalse(success);
            Assert.IsFalse(apiClient.LoggedIn);
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
            Assert.IsFalse(data.Success);

            await Login();

            data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsTrue(data.Success);

        }

        [TestMethod]
        public async Task PutAsync()
        {
            await Login();
            var data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsNotNull(data.Data);
            int submissionId = data.Data.ID;

            submission.Content = "Update content test";
            serializedSubmission = new HttpStringContent(JsonConvert.SerializeObject(submission));

            var submissionUri = new Uri(TestSettings.FullApiPath + "v/Test/" + submissionId);

            var putData = await apiClient.PutAsync<ApiResponse<ApiSubmission>>(submissionUri, serializedSubmission);
            Assert.IsNotNull(putData.Data);
            Assert.AreEqual("Update content test", putData.Data.Content);
        }

        [TestMethod]
        public async Task DeleteAsync()
        {
            await Login();
            var data = await apiClient.PostAsync<ApiResponse<ApiSubmission>>(submissionListUri, serializedSubmission);
            Assert.IsNotNull(data.Data);
            int submissionId = data.Data.ID;
            var submissionUri = new Uri(TestSettings.FullApiPath + "v/Test/" + submissionId);
            var deleteData = await apiClient.DeleteAsync<ApiResponse>(submissionUri);
            Assert.IsTrue(deleteData.Success);
        }
    }
}
