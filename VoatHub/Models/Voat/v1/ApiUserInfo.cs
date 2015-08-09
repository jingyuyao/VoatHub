﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;

namespace VoatHub.Models.Voat.v1
{
    public class ApiUserInfo
    {
        ///// <summary>
        ///// Path of avatar file
        ///// </summary>
        //[JsonProperty("avatar")]
        //[DataMember(Name = "avatar")]
        //public string Avatar { get; set; }

        /// <summary>
        /// The user name of the user when addressed by name
        /// </summary>
        [JsonProperty("userName")]
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// Date user registered
        /// </summary>
        [JsonProperty("registrationDate")]
        [DataMember(Name = "registrationDate")]
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Short bio of user
        /// </summary>
        [JsonProperty("bio")]
        [DataMember(Name = "bio")]
        public string Bio { get; set; }

        /// <summary>
        /// Path to profile picture
        /// </summary>
        [JsonProperty("profilePicture")]
        [DataMember(Name = "profilePicture")]
        public string ProfilePicture { get; set; }

        ///// <summary>
        ///// Comment Contribution Points
        ///// </summary>
        //public int CCP { get; set; }

        ///// <summary>
        ///// Submission Contribution Points
        ///// </summary>
        //public int SCP { get; set; }


        /// <summary>
        /// Comment Contribution Points (CCP)
        /// </summary>
        [JsonProperty("commentPoints")]
        [DataMember(Name = "commentPoints")]
        public Score CommentPoints { get; set; }

        /// <summary>
        /// Submission Contribution Points (SCP)
        /// </summary>
        [JsonProperty("submissionPoints")]
        [DataMember(Name = "submissionPoints")]
        public Score SubmissionPoints { get; set; }

        /// <summary>
        /// Comment Voting Behavior (Only available if request is authenticated)
        /// </summary>
        [JsonProperty("commentVoting")]
        [DataMember(Name = "commentVoting")]
        public Score CommentVoting { get; set; }

        /// <summary>
        /// Submission Voting Distribution (Only available if request is authenticated)
        /// </summary>
        [JsonProperty("submissionVoting")]
        [DataMember(Name = "submissionVoting")]
        public Score SubmissionVoting { get; set; }

        /// <summary>
        /// The badges the user has accumulated 
        /// </summary>
        [JsonProperty("badges")]
        [DataMember(Name = "badges")]
        public List<ApiUserBadge> Badges { get; set; }
    }
}
