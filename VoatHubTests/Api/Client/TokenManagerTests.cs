using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VoatHub.Api;
using VoatHub.Data;

namespace VoatHubTests.Api
{
    [TestClass]
    public class TokenManagerTests
    {
        private readonly string testClientName = "someRandomTestClient";
        private TokenManager tokenManager;
        private static ApiToken goodToken;
        private static ApiToken badToken;

        [ClassInitialize]
        public static void sampleTokenSetup(TestContext context)
        {
            goodToken = new ApiToken
            {
                access_token = "longstringofrandomasubersecretletterzzzzz",
                expires_in = 300,
                token_type = "bearer",
                userName = "dummy"
            };

            badToken = new ApiToken {
                access_token = "doesn'tmatter",
                expires_in = -1,
                token_type = "bearer",
                userName = "dummy"
            };
        }

        [TestInitialize]
        public void setUp()
        {
            tokenManager = new TokenManager(testClientName);
        }

        [TestCleanup]
        public void cleanUp()
        {
            tokenManager.Clear();
        }

        [TestMethod]
        public void Initialization()
        {
            Assert.AreEqual(null, tokenManager.AccessToken);
        }

        [TestMethod]
        public void SetToken()
        {
            tokenManager.SetToken(goodToken);
            Assert.IsNotNull(tokenManager.AccessToken);
        }

        [TestMethod]
        public void Clear()
        {
            tokenManager.SetToken(goodToken);
            Assert.IsNotNull(tokenManager.AccessToken);
            tokenManager.Clear();
            Assert.IsNull(tokenManager.AccessToken);
        }

        [TestMethod]
        public void Expire()
        {
            tokenManager.SetToken(goodToken);
            Assert.IsFalse(tokenManager.Expired);
            
            tokenManager.SetToken(badToken);
            Assert.IsTrue(tokenManager.Expired);
        }
    }
}
