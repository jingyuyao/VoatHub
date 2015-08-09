using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace VoatHub.Models.Voat.v1
{
    public class ApiSubscription
    {
        /// <summary>
        /// Specifies the type of subscription. 
        /// </summary>
        [JsonProperty("type")]
        [DataMember(Name = "type")]
        public SubscriptionType Type { get; set; }

        /// <summary>
        /// The friendly name of the subscription
        /// </summary>
        [JsonProperty("typeName")]
        [DataMember(Name = "typeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// Specifies the name of the subscription item.
        /// </summary>
        [JsonProperty("name")]
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    public enum SubscriptionType
    {

        /// <summary>
        /// Represents a subverse subscription
        /// </summary>
        Subverse = 1,
        /// <summary>
        /// Represents a set subscription
        /// </summary>
        Set = 2
    }
}
