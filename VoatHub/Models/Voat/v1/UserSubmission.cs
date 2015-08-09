using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat
{
    public class UserSubmission
    {
        /// <summary>
        /// The title for a post. This value is editable within a 10 minute window, afterwards title edits are ignored.
        /// </summary>
        //[Required]
        [DataMember(Name = "title")]
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Not Implemented. Specifies if the submission is NSFW or not.
        /// </summary>
        [JsonProperty("nsfw")]
        [DataMember(Name = "nsfw")]
        public bool Nsfw { get; set; }

        /// <summary>
        /// Not Implemented. Specifies if the submission is Anonymous or not.
        /// </summary>
        [JsonProperty("anon")]
        [DataMember(Name = "anon")]
        public bool Anonymous { get; set; }

        /// <summary>
        /// Optional. A value containing the url for a link submission. If this value is set, content is ignored.  Not-Editable once saved.
        /// </summary>
        [JsonProperty("url")]
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional. A value containing the content/text for a submission. Editable for self-posts only.
        /// </summary>
        [DataMember(Name = "content")]
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// I think they forgot to comment this out...
        /// </summary>
        public bool HasState { get; set; }
    }
}
