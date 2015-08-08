using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VoatHub.Api;
using VoatHub.Data;

namespace VoatHubTests.Api
{
    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void ToQueryStringTest()
        {
            Assert.AreEqual(Utility.ToQueryString(null), "");

            var options = new SearchOptions();
            options.count = 1;

            Assert.AreEqual("?count=1", Utility.ToQueryString(options));

            options.sort = SortAlgorithm.Hot;
            Assert.AreEqual("?sort=" + (int)SortAlgorithm.Hot + "&count=1", Utility.ToQueryString(options));
        }
    }
}
