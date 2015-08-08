using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VoatHub.Api.Client;

namespace VoatHubTests.Api.Client
{
    [TestClass]
    public class ThrottleManagerTests
    {
        private static ThrottleManager throttleManager;
        /// <summary>
        /// Allows throttle tests to run quickly.
        /// </summary>
        private static TimeSpan testTimeSpan = TimeSpan.FromMilliseconds(100);

        [TestInitialize]
        public void setUp()
        {
            throttleManager = new ThrottleManager(testTimeSpan);
        }

        [TestMethod]
        public async Task MadeCall()
        {
            var beforeWait = DateTime.Now;
            throttleManager.MadeCall();
            await throttleManager.Wait();
            var dt = DateTime.Now.Subtract(beforeWait);
            Assert.IsTrue(dt >= testTimeSpan);
        }

        [TestMethod]
        public async Task Wait()
        {
            var beforeWait = DateTime.Now;
            await throttleManager.Wait();
            // Assumes machine can run two line of code in under 100 milisecond
            Assert.IsTrue(DateTime.Now.Subtract(beforeWait) <= testTimeSpan);
        }
    }
}
