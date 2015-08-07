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
        readonly string tokenUri = "https://fakevout.azurewebsites.net/api/token";

        TokenManager tokenManager;
        CredentialManager credentialManager;
        HttpClient httpClient;

        private void setUp()
        {
            credentialManager = new CredentialManager("tokenManagerTest");
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", "ZbDlC73ndD6TB84WQmKvMA==");
        }

        [TestMethod]
        public async Task InitializationTest()
        {
            setUp();
            credentialManager.Logout();
            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            Assert.AreEqual(await tokenManager.AccessToken(), null);
        }

        [TestMethod]
        public async Task TokenTest()
        {
            setUp();
            credentialManager.Login("swampfire100", "password");

            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            Assert.AreNotEqual(await tokenManager.AccessToken(), null);
        }
    }
}
