using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace VoatHubTests.Api
{
    [TestClass]
    public class TestSettings
    {
        public static readonly string Username = "swampfire100";
        public static readonly string Password = "password";
        public static readonly string ApiKey = "ZbDlC73ndD6TB84WQmKvMA==";
        public static readonly string Scheme = "https";
        public static readonly string Host = "fakevout.azurewebsites.net";
        public static readonly string ApiPath = "api/v1/";
        public static readonly string FullApiPath = Scheme + "://" + Host + "/" + ApiPath;
        public static readonly string TokenUri = "https://fakevout.azurewebsites.net/api/token";
    }
}
