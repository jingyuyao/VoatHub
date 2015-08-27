using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VoatHub.Models.Voat.v1;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.VoatHub.LoadingList;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// An obversable model for ApiSubmission.
    /// </summary>
    public class DetailPageVM : BindableBase
    {
        private VoatApi VOAT_API = App.VOAT_API;

        public DetailPageVM(ApiSubmission submission, bool forceShowComments)
        {
            CommentSort = "Hot";
            ShowComments = forceShowComments;
            Submission = submission;
        }

        #region Properties

        private ApiSubmission _Submission;
        public ApiSubmission Submission
        {
            get { return _Submission; }
            set { SetProperty(ref _Submission, value); }
        }

        private bool _ShowComments;
        public bool ShowComments
        {
            get { return _ShowComments; }
            set { SetProperty(ref _ShowComments, value); }
        }

        private string _CommentSort;
        public string CommentSort
        {
            get { return _CommentSort; }
            set { SetProperty(ref _CommentSort, value); }
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

        #region Method

        public override string ToString()
        {
            return Submission.ToString();
        }
        
        #endregion
    }
}
