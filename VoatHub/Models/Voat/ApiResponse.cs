using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        /// <summary>
        /// Ignores any serialization error that might occur. This enable us to already return an ApiResponse Instance
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorContext"></param>
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
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
