using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub.LoadingList;

namespace VoatHub.Models.VoatHub
{
    public class SubmissionCommentsVM : BindableBase
    {
        private VoatApi VOAT_API = App.VOAT_API;

        public SubmissionCommentsVM(SubmissionVM vm)
        {
            SubmissionVM = vm;
            VOAT_API.ResetCommentPage();
            CommentSort = VOAT_API.CommentSearchOptions.sort.ToString();
            _Comments = new LoadingList<CommentVM>();
            LoadComments();
        }

        ~SubmissionCommentsVM()
        {
            Debug.WriteLine("~SubmissionCommentsVM()");
        }

        #region Properties
        private SubmissionVM _SubmissionVM;
        public SubmissionVM SubmissionVM
        {
            get { return _SubmissionVM; }
            set { SetProperty(ref _SubmissionVM, value); }
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

        private LoadingList<CommentVM> _Comments;
        public LoadingList<CommentVM> Comments
        {
            get { return _Comments; }
        }

        private bool _HasMoreComments;
        public bool HasMoreComments
        {
            get { return _HasMoreComments; }
            set { SetProperty(ref _HasMoreComments, value); }
        }
        #endregion
        
        /// <summary>
        /// Used for load and reloading comments.
        /// </summary>
        public async void LoadComments()
        {
            Comments.Dispose();
            var submission = SubmissionVM.Submission;
            var response = await VOAT_API.GetCommentList(submission.Subverse, submission.ID);
            if (response.Success)
            {
                if (submission.CommentCount > response.Data.Count) HasMoreComments = true;

                // TODO: Shit man, we going through the list at least 3 times. Might have to write more
                // specific code if performance becomes a problem.
                var commentVMList = CommentVM.FromApiCommentList(response.Data);

                foreach (var vm in commentVMList)
                {
                    Comments.List.Add(vm);
                }
            }

            Comments.Loading = false;
        }

        public async void SendSubmissionReply()
        {
            var submission = SubmissionVM.Submission;
            ReplyOpen = false;

            var value = new UserValue { Value = ReplyText };
            var r = await VOAT_API.PostComment(submission.Subverse, submission.ID, value);

            if (r.Success)
            {
                var newComment = r.Data;
                newComment.Level = 0;
                Comments.List.Insert(0, new CommentVM(newComment));
                ReplyText = "";
            }
        }
    }
}
