using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat.v1
{
    /// <summary>
    /// Container for information pertaining to a subverse.
    /// </summary>
    /// <remarks>http://fakevout.azurewebsites.net/api/help/resourcemodel?modelName=ApiSubverseInfo</remarks>
    public class ApiSubverseInfo
    {
        [JsonProperty("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }


        [JsonProperty("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        [DataMember(Name = "description")]
        public string Description { get; set; }


        [JsonProperty("creationDate")]
        [DataMember(Name = "creationDate")]
        public DateTime CreationDate { get; set; }


        [JsonProperty("subscriberCount")]
        [DataMember(Name = "subscriberCount")]
        public int SubscriberCount { get; set; }


        [JsonProperty("ratedAdult")]
        [DataMember(Name = "ratedAdult")]
        public Nullable<bool> RatedAdult { get; set; }

        [JsonProperty("sidebar")]
        [DataMember(Name = "sidebar")]
        public string Sidebar { get; set; }


        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
