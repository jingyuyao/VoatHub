using System;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace VoatHub.Models.Voat.v1
{
    public class ApiUserBadge
    {
        [JsonProperty("name")]
        [DataMember(Name = "name")]
        public string BadgeName { get; set; }

        [JsonProperty("awardedDate")]
        [DataMember(Name = "awardedDate")]
        public DateTime Awarded { get; set; }

        [JsonProperty("title")]
        [DataMember(Name = "title")]
        public string BadgeTitle { get; set; }

        [JsonProperty("badgeGraphic")]
        [DataMember(Name = "badgeGraphic")]
        public string BadgeGraphics { get; set; }
    }
}
