using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

using VoatHub.Api.Voat;
using VoatHub.Data.Voat;

namespace VoatHubTests.Api
{
    [TestClass]
    public class VoatApiTests
    {
        private static VoatApi api = new VoatApi(TestSettings.ApiKey, TestSettings.Scheme, TestSettings.Host, TestSettings.ApiPath, TestSettings.TokenUri);
        private static UserSubmission sampleSubmission = new UserSubmission
        {
            title = "Test test test test!!!",
            nsfw = false,
            anon = false,
            content = "Lets hope this works. " + DateTime.Now.ToString(),
            HasState = true
        };

        private static ApiUserPreferences samplePerf = new ApiUserPreferences
        {
            disableCustomCSS = false,
            enableAdultContent = true,
            openLinksNewWindow = true,
            publiclyDisplaySubscriptions = false,
            language = "french",
            enableNightMode = false,
            publiclyDisplayVotes = false,
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
        public async Task PostSubmission()
        {

            var response = await api.PostSubmission("Test", sampleSubmission);
            Assert.IsInstanceOfType(response, typeof(ApiResponse<ApiSubmission>));
        }

        [TestMethod]
        public async Task GetSubmissionList()
        {
            var submissions = await api.GetSubmissionList("Test", null);
            Assert.IsInstanceOfType(submissions, typeof(ApiResponse<List<ApiSubmission>>));
            Assert.IsInstanceOfType(submissions.data, typeof(List<ApiSubmission>));
            Assert.IsTrue(submissions.success);
            Assert.IsNull(submissions.error);
        }

        [TestMethod]
        public async Task GetSubmission()
        {
            var submission = await api.GetSubmission("Test", 1);
            // Heh
            Assert.AreEqual("Wow, Voat on Azure is a pain in the...", submission.data.title);

            submission = await api.GetSubmission(1);
            Assert.IsTrue(submission.success);
        }

        [TestMethod]
        public async Task EditSubmission()
        {
            var response = await api.PostSubmission("Test", sampleSubmission);
            int submissionId = response.data.id;
            string newContent = DateTime.Now.ToString();
            sampleSubmission.content = newContent;
            response = await api.PutSubmission(submissionId, sampleSubmission);
            Assert.IsTrue(response.success);
            Assert.AreEqual(newContent, response.data.content);

            var deletedResponse = await api.DeleteSubmission(submissionId);
            Assert.IsTrue(response.success);
        }

        [TestMethod]
        public async Task UserPerferences()
        {
            // Internal server error?!
            //var response = await api.PutPreferences(samplePerf);
            //Assert.IsTrue(response.success);
            var preferences = await api.GetPreferences();
            Assert.IsTrue(preferences.success);
        }
    }
}
