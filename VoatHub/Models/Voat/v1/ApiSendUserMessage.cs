using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat.v1
{
    public class ApiSendUserMessage
    {

        [RequiredAttribute]
        [JsonProperty("recipient")]
        [DataMember(Name = "recipient")]
        public string Recipient { get; set; }

        [Required]
        [MaxLength(50)]
        [JsonProperty("subject")]
        [DataMember(Name = "subject")]
        public string Subject { get; set; }

        [Required]
        [JsonProperty("message")]
        [DataMember(Name = "message")]
        public string Message { get; set; }

    }
}