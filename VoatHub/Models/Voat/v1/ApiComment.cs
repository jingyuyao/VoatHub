﻿/*
This source file is subject to version 3 of the GPL license, 
that is bundled with this package in the file LICENSE, and is 
available online at http://www.gnu.org/licenses/gpl.txt; 
you may not use this file except in compliance with the License. 

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

All portions of the code written by Voat are Copyright (c) 2014 Voat
All Rights Reserved.
*/

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace VoatHub.Models.Voat.v1
{
    public class ApiComment : FormattedContentContainer
    {
        /// <summary>
        /// Since we defined a copy constructor we need to define the empty constructor
        /// so the compiler don't always use the copy constructor.
        /// </summary>
        public ApiComment()
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="comment"></param>
        public ApiComment(ApiComment comment)
        {
            if (comment != null)
            {
                Content = comment.Content;
                FormattedContent = comment.FormattedContent;
                ID = comment.ID;
                ParentID = comment.ParentID;
                SubmissionID = comment.SubmissionID;
                Subverse = comment.Subverse;
                Date = comment.Date;
                LastEditDate = comment.LastEditDate;
                UpVotes = comment.UpVotes;
                DownVotes = comment.DownVotes;
                UserName = comment.UserName;
                ChildCount = comment.ChildCount;
                Level = comment.Level;
            }
        }


        /// <summary>
        /// The comment ID.
        /// </summary>
        [JsonProperty("id")]
        [DataMember(Name = "id")]
        public int ID { get; set; }

        /// <summary>
        /// The parent comment ID. If null then comment is a root comment.
        /// </summary>
        [JsonProperty("parentID")]
        [DataMember(Name = "parentID")]
        public Nullable<int> ParentID { get; set; }

        /// <summary>
        /// The submission ID that this comment belongs.
        /// </summary>
        [JsonProperty("submissionID")]
        [DataMember(Name = "submissionID")]
        public Nullable<int> SubmissionID { get; set; }

        /// <summary>
        /// The subveres that this comment belongs.
        /// </summary>
        [JsonProperty("subverse")]
        [DataMember(Name = "subverse")]
        public string Subverse { get; set; }

        /// <summary>
        /// Date comment was submitted.
        /// </summary>
        [JsonProperty("date")]
        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Date comment was edited.
        /// </summary>
        [JsonProperty("lastEditDate")]
        [DataMember(Name = "lastEditDate")]
        public Nullable<DateTime> LastEditDate { get; set; }

        /// <summary>
        /// The upvote count of the comment.
        /// </summary>
        [JsonProperty("upVotes")]
        [DataMember(Name = "upVotes")]
        public int UpVotes { get; set; }

        /// <summary>
        /// The downvote count of the comment.
        /// </summary>
        [JsonProperty("downVotes")]
        [DataMember(Name = "downVotes")]
        public int DownVotes { get; set; }

        /// <summary>
        /// The user name who submitted the comment.
        /// </summary>
        [JsonProperty("userName")]
        [DataMember(Name = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// Child comment count. This is a count of direct decedents only. 
        /// </summary>
        [JsonProperty("childCount")]
        [DataMember(Name = "childCount")]
        public int? ChildCount { get; set; }

        /// <summary>
        /// Level of the comment. 0 is root. This value is relative to the parent comment. If you are loading mid-branch 0 will be returned for the starting position comment.
        /// </summary>
        [JsonProperty("level")]
        [DataMember(Name = "level")]
        public int? Level { get; set; }

        // Custom

        [JsonIgnore]
        [IgnoreDataMember]
        public int TotalVotes
        {
            get
            {
                return UpVotes - DownVotes;
            }
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Sub: {1}, User: {2}, Level: {3}, TotalVotes: {4}, ChildCount: {5}", ID, Subverse, UserName, Level, TotalVotes, ChildCount);
        }
    }
}