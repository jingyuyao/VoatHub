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
    public class SubmissionCommentsVM : SubmissionVM
    {
        private VoatApi VOAT_API = App.VOAT_API;

        public SubmissionCommentsVM(SubmissionVM vm) : base(vm)
        {
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

            var response = await VOAT_API.GetCommentList(Submission.Subverse, Submission.ID);
            if (response.Success)
            {
                if (Submission.CommentCount > response.Data.Count) HasMoreComments = true;

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
            ReplyOpen = false;

            var value = new UserValue { Value = ReplyText };
            var r = await VOAT_API.PostComment(Submission.Subverse, Submission.ID, value);

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
