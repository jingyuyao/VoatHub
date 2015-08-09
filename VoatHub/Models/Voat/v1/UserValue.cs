using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat
{
    public class UserValue
    {
        /// <summary>
        /// Content of request
        /// </summary>
        [DataMember(Name = "value")]
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
