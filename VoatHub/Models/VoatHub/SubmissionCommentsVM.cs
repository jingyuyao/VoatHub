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
            VOAT_API.CommentSearchOptions.page = 1;

            // TODO: Load from previous session
            var apiCommentSort = VOAT_API.CommentSearchOptions.sort;

            if (apiCommentSort == null)
                CommentSort = "Hot";
            else
                CommentSort = apiCommentSort.ToString();
            
            Comments = new LoadingList<CommentTree>();
            LoadComments();
        }

        ~SubmissionCommentsVM()
        {
            Debug.WriteLine("SubmissionCommentsVM destroyed");
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

        private LoadingList<CommentTree> _Comments;
        public LoadingList<CommentTree> Comments
        {
            get { return _Comments; }
            set { SetProperty(ref _Comments, value); }
        }

        private bool _HasMoreComments;
        public bool HasMoreComments
        {
            get { return _HasMoreComments; }
            set { SetProperty(ref _HasMoreComments, value); }
        }

        /// <summary>
        /// This method is kept private because somehow making bulk changes to the comments collection causes
        /// some painfully heavy memory problems. The problem might be caused by some redrawing or the visual
        /// tree or some other xaml bullshit. So to change the comments collection you just have to recreate
        /// the model/page. This is a temporary solution until I can find the root casue of the memory problem
        /// </summary>
        private async void LoadComments()
        {
            var response = await VOAT_API.GetCommentList(Submission.Subverse, Submission.ID);
            if (response.Success)
            {
                // TODO: Shit man, we going through the list at least 3 times. Might have to write more
                // specific code if performance becomes a problem.
                var commentTreeList = CommentTree.FromApiCommentList(response.Data, null);
                var sortedList = commentTreeSorter(commentTreeList);
                if (Submission.CommentCount > response.Data.Count) HasMoreComments = true;

                Comments.List = sortedList;
            }

            Comments.Loading = false;
        }

        /// <summary>
        /// Sorts the commentTreeList based on <see cref="commentSearchOptions"/>.
        /// <para>Expensive operation</para>
        /// </summary>
        /// <param name="commentTree"></param>
        private ObservableCollection<CommentTree> commentTreeSorter(ObservableCollection<CommentTree> commentTreeList)
        {
            switch (VOAT_API.CommentSearchOptions.sort)
            {
                case SortAlgorithm.New:
                    return CommentTree.SortNew(commentTreeList);
                case SortAlgorithm.Top:
                    return CommentTree.SortTop(commentTreeList);
            }

            return commentTreeList; ;
        }

        public async void SendReply()
        {
            var value = new UserValue { Value = ReplyText };
            var r = await VOAT_API.PostComment(Submission.Subverse, Submission.ID, value);

            if (r.Success)
            {
                var newComment = r.Data;
                newComment.Level = 0;
                Comments.List.Add(new CommentTree(newComment));
            }
        }
    }
}
