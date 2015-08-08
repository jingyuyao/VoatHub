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
        static VoatApi api = new VoatApi(TestSettings.ApiKey, TestSettings.BaseUri, TestSettings.TokenUri);

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

            bool loggedIn = await api.Login("swampfire100", "password");
            Assert.IsTrue(loggedIn);

            var response = await api.PostSubmission("Test", submission);
            Assert.IsInstanceOfType(response, typeof(ApiResponse<ApiSubmission>));
        }

        [TestMethod]
        public async Task GetSubmissionList()
        {
            var submissions = await api.GetSubmissionList("Test", null);
            Assert.IsInstanceOfType(submissions, typeof(ApiResponse<List<ApiSubmission>>));
            Assert.IsInstanceOfType(submissions.data, typeof(List<ApiSubmission>));
            Assert.AreEqual(true, submissions.success);
            Assert.AreEqual(null, submissions.error);
        }

        [TestMethod]
        public async Task GetSubmission()
        {
            var submission = await api.GetSubmission("Test", 1);
            // Heh
            Assert.AreEqual("Wow, Voat on Azure is a pain in the...", submission.data.title);
        }

        [TestMethod]
        public void VoatApiDisposeTest()
        {
            api.Dispose();
        }
    }
}
