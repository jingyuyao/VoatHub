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
        public List<CommentTree> CommentTree { get; set; }
        public bool LoadingComments { get; set; }

        public override string ToString()
        {
            var count = CommentTree == null ? 0 : CommentTree.Count;
            return Submission.ToString() + " Comment Count:" + count;
        }
    }
}
