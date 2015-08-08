using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VoatHub.Api.Client;

namespace VoatHubTests.Api
{
    [TestClass]
    public class CredentialManagerTests
    {
        CredentialManager credentialManager;
        string clientName = "voatTest";
        string username = "tester";
        string password = "password";

        [TestInitialize]
        public void setUp()
        {
            credentialManager = new CredentialManager(clientName);
        }

        [TestCleanup]
        public void cleanUp()
        {
            credentialManager.Logout();
        }

        [TestMethod]
        public void Initialization()
        {
            Assert.AreEqual(null, credentialManager.Credential);
        }

        [TestMethod]
        public void Login()
        {
            credentialManager.Login(username, password);
            Assert.IsTrue(credentialManager.LoggedIn);
            Assert.IsNotNull(credentialManager.Credential);
            Assert.AreEqual(username, credentialManager.Credential.UserName);
            Assert.AreEqual(password, credentialManager.Credential.Password);
        }

        [TestMethod]
        public void Logout()
        {
            Login();
            credentialManager.Logout();
            Assert.IsFalse(credentialManager.LoggedIn);
            Assert.IsNull(credentialManager.Credential);
        }
    }
}
