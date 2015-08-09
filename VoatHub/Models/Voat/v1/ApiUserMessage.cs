using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VoatHub.Models.Voat.v1
{
    public class ApiUserMessage
    {

        [JsonProperty("messageID")]
        [DataMember(Name = "messageID")]
        public int MessageID { get; set; }

        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [JsonProperty("sender")]
        [DataMember(Name = "sender")]
        public string Sender { get; set; }

        [JsonProperty("message")]
        [DataMember(Name = "message")]
        public string Message { get; set; }

    }
}
