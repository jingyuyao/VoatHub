using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Windows.Web.Http;

using VoatHub.Api;

namespace VoatHubTests.Api
{
    [TestClass]
    public class TokenManagerTests
    {
        readonly string TOKEN_URI = "https://fakevout.azurewebsites.net/api/token";
        readonly string API_KEY = "ZbDlC73ndD6TB84WQmKvMA==";

        [TestMethod]
        public async Task InitializationTest()
        {
            var client = new VoatApiClient(API_KEY, TOKEN_URI);
            var tokenManager = new TokenManager(client);
            Assert.AreEqual(await tokenManager.AccessToken(), null);
        }

        [TestMethod]
        public async Task TokenTest()
        {
            var client = new VoatApiClient(API_KEY, TOKEN_URI);
            client.Login("swampfire100", "password");

            var tokenManager = new TokenManager(client);
            Assert.AreNotEqual(await tokenManager.AccessToken(), null);
            client.Logout();
        }
    }
}
