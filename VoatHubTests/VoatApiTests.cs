﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

using VoatHub.Api;
using VoatHub.Data;

namespace VoatHubTests
{
    [TestClass]
    public class VoatApiTests
    {
        VoatApi api;
        readonly string apiKey = "ZbDlC73ndD6TB84WQmKvMA==";
        readonly string baseUri = "https://fakevout.azurewebsites.net/api/v1/";

        [TestMethod]
        public async Task GetSubmissions()
        {
            api = new VoatApi(apiKey, baseUri);
            var submissions = await api.GetSubmissions("Test");
            Assert.IsInstanceOfType(submissions, typeof(ApiResponse<List<ApiSubmission>>));
            Assert.IsInstanceOfType(submissions.data, typeof(List<ApiSubmission>));
            Assert.AreNotEqual(submissions.data.Count, 0);
            Assert.AreEqual(submissions.success, true);
            Assert.AreEqual(submissions.error, null);

            foreach (var submission in submissions.data)
            {
                verifySubmission(submission);
            }

            api.Dispose();
        }

        /// <summary>
        /// Temporary for verifying my sanity
        /// </summary>
        /// <param name="submission"></param>
        private void verifySubmission(ApiSubmission submission)
        {
            Assert.IsNotNull(submission.id);
            Assert.IsNotNull(submission.date);
        }
    }
}