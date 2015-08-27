using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub.LoadingList;

namespace VoatHub.Models.VoatHub
{
    public class CommentsVM : BindableBase
    {
        private VoatApi VOAT_API = App.VOAT_API;

        public CommentsVM()
        {
            // Fixes item source null binding errors.
            Comments = new LoadingList<CommentTree>();
        }

        public CommentsVM(ApiSubmission submission) : this()
        {
            loadSubmissionComments(submission);
        }

        public CommentsVM(ApiComment comment) : this()
        {
            // TODO: load child comments
            throw new NotImplementedException();
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

        private async void loadSubmissionComments(ApiSubmission submission)
        {
            var response = await VOAT_API.GetCommentList(submission.Subverse, submission.ID);
            if (response.Success)
            {
                // TODO: Shit man, we going through the list at least 3 times. Might have to write more
                // specific code if performance becomes a problem.
                var commentTreeList = CommentTree.FromApiCommentList(response.Data, null);
                var sortedList = commentTreeSorter(commentTreeList);
                Comments.List = sortedList;
                if (submission.CommentCount > CommentTree.Count(sortedList)) HasMoreComments = true;
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
    }
}
