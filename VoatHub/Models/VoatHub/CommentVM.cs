using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Ui;
using Windows.UI.Xaml;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// A ViewModel for comments.
    /// </summary>
    public class CommentVM : BindableBase
    {
        private static readonly VoatApi VOAT_API = App.VOAT_API;
        private static readonly int INDENTATION_FACTOR = 10;

        /// <summary>
        /// Invariant: never null.
        /// </summary>
        private List<CommentVM> children;

        /// <summary>
        /// Keeps track of visibility state changes so we can go back to it once we
        /// 'open up' the parent block
        /// </summary>
        private Stack<Visibility> previousParentVisibility;

        public CommentVM(ApiComment comment)
        {
            if (comment == null) throw new ArgumentException("Comment cannot be null.");

            Comment = comment;
            children = new List<CommentVM>();
            SelfVisibility = Visibility.Visible;
            ParentVisibility = Visibility.Visible;
            previousParentVisibility = new Stack<Visibility>();
            int indentValue = Comment.Level == null ? 0 : (int)Comment.Level * INDENTATION_FACTOR;
            Indentation = new Thickness(indentValue, 0, 0, 0);

            ReplyOpen = false;
        }

        #region Properties
        /// <summary>
        /// Invariant: never null.
        /// </summary>
        private ApiComment _Comment;
        public ApiComment Comment
        {
            get { return _Comment; }
            set { SetProperty(ref _Comment, value); }
        }

        private Thickness _Indentation;
        public Thickness Indentation
        {
            get { return _Indentation; }
            set { SetProperty(ref _Indentation, value); }
        }

        private Visibility _SelfVisibility;
        public Visibility SelfVisibility
        {
            get { return _SelfVisibility; }
            set { SetProperty(ref _SelfVisibility, value); }
        }

        private Visibility _ParentVisibility;
        public Visibility ParentVisibility
        {
            get { return _ParentVisibility; }
            set { SetProperty(ref _ParentVisibility, value); }
        }

        private bool? _ReplyOpen;
        public bool? ReplyOpen
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

        #region PrivateMethods
        private void HideChildren()
        {
            foreach (var child in children)
            {
                child.previousParentVisibility.Push(child.ParentVisibility);
                child.ParentVisibility = Visibility.Collapsed;
                child.HideChildren();
            }
        }

        private void RestoreChildrenVisibility()
        {
            foreach (var child in children)
            {
                if (child.previousParentVisibility.Count != 0)
                {
                    child.ParentVisibility = child.previousParentVisibility.Pop();
                }

                child.RestoreChildrenVisibility();
            }
        }

        private static CommentVM FindParentList(ApiComment target, IList<CommentVM> source)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var result = source[i].FindParent(target);
                if (result != null) return result;
            }

            return null;
        }

        /// <summary>
        /// Assumptions: Comment.Level is accurate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private CommentVM FindParent(ApiComment target)
        {
            // Optomization: Parent's level must be less than target's level
            if (Comment.Level >= target.Level) return null;

            if (target.ParentID != null && Comment.ID == target.ParentID) return this;

            return FindParentList(target, children);
        }

        private static void Flatten(List<CommentVM> list, List<CommentVM> flattened)
        {
            foreach (var vm in list)
            {
                Flatten(vm, flattened);
            }
        }

        private static void Flatten(CommentVM vm, List<CommentVM> flattened)
        {
            flattened.Add(vm);
            Flatten(vm.children, flattened);
        }

        private static void insertUsingComparer(CommentVM vm, List<CommentVM> list, IComparer<CommentVM> comparer)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer.Compare(vm, list[i]) > 0)
                {
                    list.Insert(i, vm);
                    return;
                }
            }

            list.Add(vm);
        }

        private class DefaultComparer : IComparer<CommentVM>
        {
            public int Compare(CommentVM x, CommentVM y)
            {
                return -1;
            }
        }

        private class DateComparer : IComparer<CommentVM>
        {
            public int Compare(CommentVM x, CommentVM y)
            {
                if (x.Comment.Date > y.Comment.Date)
                    return 1;
                else if (x.Comment.Date < y.Comment.Date)
                    return -1;
                else
                    return 0;
            }
        }

        private class VoteComparer : IComparer<CommentVM>
        {
            public int Compare(CommentVM x, CommentVM y)
            {
                if (x.Comment.TotalVotes > y.Comment.TotalVotes)
                    return 1;
                else if (x.Comment.TotalVotes < y.Comment.TotalVotes)
                    return -1;
                else
                    return 0;
            }
        }
        #endregion

        #region Methods
        public async void UpVote()
        {
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", _Comment.ID, 1, true);
        }

        public async void DownVote()
        {
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", _Comment.ID, -1, true);
        }

        public async Task<CommentVM> SendReply()
        {
            ReplyOpen = false;

            var value = new UserValue { Value = ReplyText };
            var r = await VOAT_API.PostCommentReply(Comment.Subverse, (int)Comment.SubmissionID, Comment.ID, value);

            if (r.Success)
            {
                ReplyText = "";
                
                r.Data.Level = Comment.Level + 1;  // Fixes api not returning proper level for newly created comments
                var vm = new CommentVM(r.Data);

                children.Add(vm);
                return vm;
            }

            return null;
        }

        public void ToggleVisibility()
        {
            SelfVisibility = SelfVisibility.Inverse();

            if (SelfVisibility == Visibility.Collapsed)
            {
                HideChildren();
            }
            else
            {
                RestoreChildrenVisibility();
            }
        }

        public static List<CommentVM> FromApiCommentList(List<ApiComment> apiComments)
        {
            IComparer<CommentVM> comparer;

            var sort = VOAT_API.CommentSearchOptions.sort;
            switch (sort)
            {
                case SortAlgorithm.New:
                    comparer = new DateComparer();
                    break;
                case SortAlgorithm.Top:
                    comparer = new VoteComparer();
                    break;
                default:
                    comparer = new DefaultComparer();
                    break;
            }

            var rootNestedComments = new List<CommentVM>();

            foreach (var apiComment in apiComments)
            {
                var nestedComment = new CommentVM(apiComment);

                var parentComment = FindParentList(apiComment, rootNestedComments);

                if (parentComment != null)
                {
                    insertUsingComparer(nestedComment, parentComment.children, comparer);
                }
                else
                {
                    insertUsingComparer(nestedComment, rootNestedComments, comparer);
                }
            }

            var flattened = new List<CommentVM>();
            Flatten(rootNestedComments, flattened);

            return flattened;
        }
        #endregion

        public override string ToString()
        {
            return _Comment.ToString();
        }
    }
}
