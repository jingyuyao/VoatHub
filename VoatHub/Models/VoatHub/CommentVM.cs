using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using Windows.UI.Xaml;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// A ViewModel for comments.
    /// </summary>
    public class CommentVM : BindableBase
    {
        private static readonly VoatApi VOAT_API = App.VOAT_API;

        public CommentVM(ApiComment comment)
        {
            if (comment == null) throw new ArgumentException("Comment cannot be null.");

            Comment = comment;
            Children = new List<CommentVM>();
            SelfVisibility = Visibility.Visible;
            ParentVisibility = Visibility.Visible;

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

        /// <summary>
        /// Invariant: never null.
        /// </summary>
        private List<CommentVM> _Children;
        public List<CommentVM> Children
        {
            get { return _Children; }
            set { SetProperty(ref _Children, value); }
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
        
        #region Methods
        public async void UpVote()
        {
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", _Comment.ID, 1, true);
        }

        public async void DownVote()
        {
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", _Comment.ID, -1, true);
        }

        public async void SendReply()
        {
            ReplyOpen = false;

            var value = new UserValue { Value = ReplyText };
            var r = await VOAT_API.PostCommentReply(Comment.Subverse, (int)Comment.SubmissionID, Comment.ID, value);

            if (r.Success)
            {
                var newComment = r.Data;
                newComment.Level = Comment.Level + 1;  // Fixes api not returning proper level for newly created comments
                Children.Add(new CommentVM(newComment));
                ReplyText = "";
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
        public CommentVM FindParent(ApiComment target)
        {
            // Optomization: Parent's level must be less than target's level
            if (Comment.Level >= target.Level) return null;

            if (target.ParentID != null && Comment.ID == target.ParentID) return this;

            return FindParentList(target, Children);
        }
        #endregion

        #region StaticHelpers
        public static List<CommentVM> FromApiCommentList(List<ApiComment> apiComments)
        {
            var rootNestedComments = new List<CommentVM>();

            foreach (var apiComment in apiComments)
            {
                var nestedComment = new CommentVM(apiComment);

                var parentComment = FindParentList(apiComment, rootNestedComments);

                if (parentComment != null)
                {
                    parentComment.Children.Add(nestedComment);
                }
                else
                {
                    rootNestedComments.Add(nestedComment);
                }
            }

            return rootNestedComments;
        }

        /// <summary>
        /// Recursively sorts the list of <see cref="CommentVM"/> from oldest to newest.
        /// <para>VERY EXPENSIVE OPERATION!</para>
        /// <para>Sort so oldest goes first becaues list view displays the first item on the bottom</para>
        /// </summary>
        /// <param name="list"></param>
        public static ObservableCollection<CommentVM> SortNew(ObservableCollection<CommentVM> list)
        {
            var sortedList = new List<CommentVM>(list);

            sortedList.Sort((x, y) =>
            {
                if (x.Comment.Date > y.Comment.Date)
                    return -1;
                else if (x.Comment.Date < y.Comment.Date)
                    return 1;
                else
                    return 0;
            });

            foreach (var commentVM in sortedList)
            {
                commentVM.Children = SortNew(commentVM.Children);
            }

            return new ObservableCollection<CommentVM>(sortedList);
        }

        /// <summary>
        /// Recursively sorts the list of <see cref="CommentVM"/> from least votes to most votes.
        /// <para>VERY EXPENSIVE OPERATION!</para>
        /// <para>Sort so least votes goes first becaues list view displays the first item on the bottom</para>
        /// </summary>
        /// <param name="list"></param>
        public static ObservableCollection<CommentVM> SortTop(ObservableCollection<CommentVM> list)
        {
            var sortedList = new List<CommentVM>(list);

            sortedList.Sort((x, y) =>
            {
                if (x.Comment.TotalVotes > y.Comment.TotalVotes)
                    return -1;
                else if (x.Comment.TotalVotes < y.Comment.TotalVotes)
                    return 1;
                else
                    return 0;
            });

            foreach (var commentVM in sortedList)
            {
                commentVM.Children = SortNew(commentVM.Children);
            }

            return new ObservableCollection<CommentVM>(sortedList);
        }

        /// <summary>
        /// Recursively count the number of comments in a list of <see cref="CommentVM"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int Count(ObservableCollection<CommentVM> list)
        {
            int counter = 0;

            foreach (var c in list)
            {
                counter += Count(c);
            }

            return counter;
        }

        /// <summary>
        /// Recursively count the number of comments in a <see cref="CommentVM"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static int Count(CommentVM tree)
        {
            int counter = 0;
            if (tree != null)
            {
                if (tree.Comment != null) counter++;
                if (tree.Children != null)
                {
                    counter += Count(tree.Children);
                }
            }
            return counter;
        }
        #endregion

        public override string ToString()
        {
            return _Comment.ToString();
        }
    }
}
