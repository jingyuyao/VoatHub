using System;
using System.Collections.Generic;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// An obversable SubmissionModel
    /// </summary>
    public class SubmissionViewModel : BindableBase
    {
        private ApiSubmission submission;
        private List<CommentTree> commentTree;
        private bool loadingComments;
        private bool showComments;

        public SubmissionViewModel()
        {
            // Fixes item source null binding errors.
            commentTree = new List<CommentTree>();
        }

        public ApiSubmission Submission
        {
            get { return submission; }
            set { SetProperty(ref submission, value); }
        }
        public List<CommentTree> CommentTree
        {
            get { return commentTree; }
            set { SetProperty(ref commentTree, value); }
        }
        public bool LoadingComments
        {
            get { return loadingComments; }
            set { SetProperty(ref loadingComments, value); }
        }
        public bool ShowComments
        {
            get { return showComments; }
            set { SetProperty(ref showComments, value); }
        }

        public override string ToString()
        {
            var count = CommentTree == null ? 0 : CommentTree.Count;
            return Submission.ToString() + " Comment Count:" + count;
        }
    }
}
