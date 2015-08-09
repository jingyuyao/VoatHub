using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat.v1
{
    public class ApiBadge
    {
        [JsonProperty("badgeID")]
        [DataMember(Name = "badgeID")]
        public string BadgeId { get; set; }

        [JsonProperty("badgeGraphics")]
        [DataMember(Name = "badgeGraphics")]
        public string BadgeGraphics { get; set; }

        [JsonProperty("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
