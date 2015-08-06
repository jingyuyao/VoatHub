using System;
using System.Diagnostics;
using System.Collections.Generic;
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
        public void InitializationTest()
        {
            setUp();
            credentialManager.Logout();
            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            Assert.AreEqual(tokenManager.AccessToken, null);
        }

        [TestMethod]
        public void TokenTest()
        {
            setUp();
            credentialManager.Login("swampfire100", "password");

            tokenManager = new TokenManager(tokenUri, httpClient, credentialManager);
            Assert.AreNotEqual(tokenManager.AccessToken, null);
        }
    }
}
