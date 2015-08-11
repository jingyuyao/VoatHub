using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace VoatHub.Models.Voat
{
    /// <summary>
    /// Generic response returned by the Voat API.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help</remarks>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        [JsonProperty("data")]
        [DataMember(Name = "data")]
        public T Data { get; set; }

        [JsonProperty("success")]
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        [DataMember(Name = "error")]
        public ErrorInfo Error { get; set; }
    }

    /// <summary>
    /// Generic response returned by the Voat API with no data.
    /// </summary>
    public class ApiResponse
    {
        [JsonProperty("success")]
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        [DataMember(Name = "error")]
        public ErrorInfo Error { get; set; }
    }
}
