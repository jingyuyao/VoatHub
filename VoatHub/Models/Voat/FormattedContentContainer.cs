using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat
{

    public abstract class FormattedContentContainer
    {
        /// <summary>
        /// The raw content of this item.
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Include)]
        [DataMember(Name = "content")]
        public string Content{ get; set; }

        /// <summary>
        /// The formatted (MarkDown, Voat Content Processor) content of this item. This content is typically formatted into HTML output.
        /// </summary>
        [JsonProperty("formattedContent", NullValueHandling = NullValueHandling.Include)]
        [DataMember(Name = "formattedContent")]
        public string FormattedContent { get; set; }
    }
}