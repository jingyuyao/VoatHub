using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VoatHub.Models.Voat
{
    /// <summary>
    /// Container for an error information.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ErrorInfo</remarks>
    public class ErrorInfo
    {
        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        [DataMember(Name = "message")]
        public string Message { get; set; }
    }
}
