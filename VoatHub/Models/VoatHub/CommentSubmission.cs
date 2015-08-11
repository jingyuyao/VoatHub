using System;
using System.Collections.Generic;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// A submission with comments
    /// </summary>
    public class CommentSubmission
    {
        public ApiSubmission Submission { get; set; }
        public List<ApiComment> Comments { get; set; }

        public override string ToString()
        {
            var count = Comments == null ? 0 : Comments.Count;
            return Submission.ToString() + " Comment Count:" + count;
        }
    }
}
