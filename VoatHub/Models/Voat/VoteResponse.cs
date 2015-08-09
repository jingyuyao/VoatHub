using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace VoatHub.Models.Voat
{
    public class VoteResponse
    {
        /// <summary>
        /// The users recorded vote value after the operation has completed. Use this value to verify vote operation is recorded correctly. Valid values are: -1 (down voted, 0 (revoked, unvoted), or 1 (up voted) 
        /// </summary>
        [JsonProperty("recordedValue")]
        [DataMember(Name = "recordedValue")]
        public int? RecordedValue { get; set; }

        /// <summary>
        /// A value indicating whether the operation was successfull
        /// </summary>
        [JsonProperty("success", Order = 1)]
        [DataMember(Name = "success", Order = 1)]
        public bool Success { get; set; }

        /// <summary>
        /// The result of the vote operation
        /// </summary>
        [DataMember(Name = "result", Order = 2)]
        [JsonProperty("result", Order = 2)]
        public ProcessResult Result { get; set; }

        /// <summary>
        /// The friendly name of the vote operation result
        /// </summary>
        [DataMember(Name = "resultName", Order = 3)]
        [JsonProperty("resultName", Order = 3)]
        public string ResultName { get; set; }

        /// <summary>
        /// A description with information concerning the vote result
        /// </summary>
        [DataMember(Name = "message", Order = 4)]
        [JsonProperty("message", Order = 4)]
        public string message { get; set; }
    }

    /// <summary>
    /// The result of a vote operation
    /// </summary>
    public enum ProcessResult
    {
        /// <summary>
        /// Vote operation was not processed.
        /// </summary>
        NotProcessed = 0,
        /// <summary>
        /// Vote operation was successfully recorded
        /// </summary>
        Success = 1,
        /// <summary>
        /// Vote operation was ignored by the system. Reasons usually include a duplicate vote or a vote on a non-voteable item.
        /// </summary>
        Ignored = 2,
        /// <summary>
        /// Vote operation was denied by the system. Typically this response is returned when user doesn't have the neccessary requirements to vote on item.
        /// </summary>
        Denied = 3
    }
}
