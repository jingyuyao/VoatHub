using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;

using VoatHub.Api;

namespace VoatHubTests.Api
{
    [TestClass]
    public class CredentialManagerTests
    {
        CredentialManager credentialManager;
        string clientName = "voatTest";
        string username = "tester";
        string password = "password";

        [TestMethod]
        public void CredentialManagerInitialization()
        {
            credentialManager = new CredentialManager(clientName);
            Assert.AreEqual(credentialManager.Credential, null);
        }

        [TestMethod]
        public void CredentialManagerTest()
        {
            credentialManager = new CredentialManager(clientName);
            credentialManager.Login(username, password);
            var credential = credentialManager.Credential;
            Assert.AreEqual(credential.UserName, username);
            Assert.AreEqual(credential.Password, password);
            credentialManager.Logout();

            credential = credentialManager.Credential;
            Assert.AreEqual(credential, null);
        }
    }
}
