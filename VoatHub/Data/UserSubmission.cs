using Windows.Web.Http;

using Newtonsoft.Json;

namespace VoatHub.Data
{
    public class UserSubmission
    {
        public string title { get; set; }
        public bool nsfw { get; set; }
        public bool anon { get; set; }
        public string url { get; set; }
        public string content { get; set; }
        public bool HasState { get; set; }
    }
}
