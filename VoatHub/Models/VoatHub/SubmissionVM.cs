using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VoatHub.Models.Voat.v1;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// An obversable model for ApiSubmission.
    /// </summary>
    public class SubmissionVM : BindableBase
    {
        private static readonly Uri DEFAULT_URI = new Uri("about:blank");
        private VoatApi api;

        public SubmissionVM(VoatApi api)
        {
            this.api = api;
            // Prevents WebView binding to null error.
            Uri = DEFAULT_URI;

            // Fixes item source null binding errors.
            Comments = new ObservableCollection<CommentTree>();
        }

        #region Properties

        private ApiSubmission _Submission;
        public ApiSubmission Submission
        {
            get { return _Submission; }
            set { SetProperty(ref _Submission, value); }
        }

        /// <summary>
        /// Separates Submission.Url from webview source so it doesn't bind to bad values
        /// <para>Invariant: Never null or bad value.</para>
        /// </summary>
        private Uri _Uri;
        public Uri Uri
        {
            get { return _Uri; }
            set { SetProperty(ref _Uri, value); }
        }

        private ObservableCollection<CommentTree> _Comments;
        public ObservableCollection<CommentTree> Comments
        {
            get { return _Comments; }
            set { SetProperty(ref _Comments, value); }
        }

        private bool _LoadingComments;
        public bool LoadingComments
        {
            get { return _LoadingComments; }
            set { SetProperty(ref _LoadingComments, value); }
        }

        private bool _ShowComments;
        public bool ShowComments
        {
            get { return _ShowComments; }
            set { SetProperty(ref _ShowComments, value); }
        }

        private bool _ReplyOpen;
        public bool ReplyOpen
        {
            get { return _ReplyOpen; }
            set { SetProperty(ref _ReplyOpen, value); }
        }

        private string _ReplyText;
        public string ReplyText
        {
            get { return _ReplyText; }
            set { SetProperty(ref _ReplyText, value); }
        }

        #endregion

        #region Methods

        public void ChangeSubmission(ApiSubmission submission, bool forceShowComments)
        {
            ResetVM(submission);

            if (submission != null)
            {
                if (Submission.Type == ApiSubmissionType.Self || forceShowComments)
                    LoadComments();
                else
                    LoadLink();
            }
        }

        public void ResetVM()
        {
            Submission = null;
            Comments = new ObservableCollection<CommentTree>();
            LoadingComments = false;
            ShowComments = false;
            ReplyOpen = false;
            ReplyText = null;
            Uri = DEFAULT_URI;
        }

        public void ResetVM(ApiSubmission submission)
        {
            ResetVM();
            Submission = submission;
        }
        
        public async void LoadComments()
        {
            var idLoadingCommentsFor = Submission.ID;
            ShowComments = true;
            LoadingComments = true;

            var response = await api.GetCommentList(Submission.Subverse, Submission.ID);

            // If the current submission changes, then we released control over the loading icon.
            if (Submission.ID == idLoadingCommentsFor)
            {
                if (response.Success)
                {
                    var commentTreeList = CommentTree.FromApiCommentList(response.Data, null);
                    var sortedList = commentTreeSorter(commentTreeList);
                    Comments = sortedList;
                }

                LoadingComments = false;
            }
        }

        public void LoadLink()
        {
            Uri uri;
            bool success = Uri.TryCreate(Submission.Url, UriKind.Absolute, out uri);
            if (success) Uri = uri;
        }

        public void Refresh()
        {
            ChangeSubmission(Submission, ShowComments);
        }

        /// <summary>
        /// Sorts the commentTreeList based on <see cref="commentSearchOptions"/>.
        /// <para>Expensive operation</para>
        /// </summary>
        /// <param name="commentTree"></param>
        private ObservableCollection<CommentTree> commentTreeSorter(ObservableCollection<CommentTree> commentTreeList)
        {
            switch (api.CommentSearchOptions.sort)
            {
                case SortAlgorithm.New:
                    return CommentTree.SortNew(commentTreeList);
                case SortAlgorithm.Top:
                    return CommentTree.SortTop(commentTreeList);
            }

            return commentTreeList; ;
        }

        public override string ToString()
        {
            var count = Comments == null ? 0 : Comments.Count;
            return Submission.ToString() + " Comment Count:" + count;
        }
        
        #endregion
    }
}
