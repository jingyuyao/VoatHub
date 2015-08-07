using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

using VoatHub.Api;
using VoatHub.Data;

namespace VoatHubTests.Api
{
    [TestClass]
    public class VoatApiTests
    {
        static readonly string apiKey = "ZbDlC73ndD6TB84WQmKvMA==";
        static readonly string baseUri = "https://fakevout.azurewebsites.net/api/v1/";
        static readonly string tokenUri = "https://fakevout.azurewebsites.net/api/token";

        static VoatApi api = new VoatApi(apiKey, baseUri, tokenUri);

        [TestMethod]
        public async Task PostSubmission()
        {
            var submission = new UserSubmission();
            submission.title = "Test test test test!!!";
            submission.nsfw = false;
            submission.anon = false;
            submission.url = "https://www.google.com";
            submission.content = "Lets hope this works. " + DateTime.Now.ToString();
            submission.HasState = true;

            api.Login("swampfire100", "password");

            var response = await api.PostSubmission("Test", submission);
            Assert.IsInstanceOfType(response, typeof(ApiResponse<ApiSubmission>));
        }

        [TestMethod]
        public async Task GetSubmissions()
        {
            var submissions = await api.GetSubmissions("Test");
            Assert.IsInstanceOfType(submissions, typeof(ApiResponse<List<ApiSubmission>>));
            Assert.IsInstanceOfType(submissions.data, typeof(List<ApiSubmission>));
            Assert.AreEqual(submissions.success, true);
            Assert.AreEqual(submissions.error, null);
        }

        [TestMethod]
        public void VoatApiDisposeTest()
        {
            var client = new VoatApi(apiKey, baseUri, tokenUri);
            client.Dispose();
        }
    }
}
