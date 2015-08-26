﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// A ViewModel for comments.
    /// </summary>
    public class CommentTree : BindableBase
    {
        private ApiComment comment;
        private bool? replyOpen;
        private string replyText;
        private bool? show;
        private ObservableCollection<CommentTree> children;

        public CommentTree(ApiComment comment)
        {
            if (comment == null) throw new ArgumentException("Comment cannot be null.");

            Comment = comment;
            Children = new ObservableCollection<CommentTree>();
            Show = true;
            ReplyOpen = false;
        }

        #region Properties
        /// <summary>
        /// Invariant: never null.
        /// </summary>
        public ApiComment Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }
        /// <summary>
        /// Invariant: never null.
        /// </summary>
        public ObservableCollection<CommentTree> Children
        {
            get { return children; }
            set
            {
                SetProperty(ref children, value);
                HasMoreComments = Comment.ChildCount > value.Count;  // ChildCount include direct descendents only.
                value.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ListChangedHandler);
            }
        }

        public bool? Show
        {
            get { return show; }
            set { SetProperty(ref show, value); }
        }

        public bool? ReplyOpen
        {
            get { return replyOpen; }
            set { SetProperty(ref replyOpen, value); }
        }

        public string ReplyText
        {
            get { return replyText; }
            set { SetProperty(ref replyText, value); }
        }

        private bool _HasMoreComments;
        public bool HasMoreComments
        {
            get { return _HasMoreComments; }
            set { SetProperty(ref _HasMoreComments, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Assumptions: Comment.Level is accurate.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CommentTree FindParent(ApiComment target)
        {
            // Optomization: Parent's level must be less than target's level
            if (Comment.Level >= target.Level) return null;

            if (target.ParentID != null && Comment.ID == target.ParentID) return this;

            return FindParentList(target, Children);
        }

        /// <summary>
        /// Oh boy, hopefully this isn't called a lot.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListChangedHandler(object sender, EventArgs e)
        {
            var collection = sender as ObservableCollection<CommentTree>;
            HasMoreComments = Comment.ChildCount > collection.Count; // ChildCount include direct descendents only.
        }

        public override string ToString()
        {
            return comment.ToString() + " Children count: " + Count(Children);
        }
        #endregion

        #region StaticHelpers
        private static CommentTree FindParentList(ApiComment target, ObservableCollection<CommentTree> source)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var result = source[i].FindParent(target);
                if (result != null) return result;
            }

            return null;
        }

        public static ObservableCollection<CommentTree> FromApiCommentList(List<ApiComment> apiComments, ObservableCollection<CommentTree> source)
        {
            var rootNestedComments = source ?? new ObservableCollection<CommentTree>();

            foreach (var apiComment in apiComments)
            {
                var nestedComment = new CommentTree(apiComment);

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
        /// Recursively sorts the list of <see cref="CommentTree"/> from oldest to newest.
        /// <para>VERY EXPENSIVE OPERATION!</para>
        /// <para>Sort so oldest goes first becaues list view displays the first item on the bottom</para>
        /// </summary>
        /// <param name="list"></param>
        public static ObservableCollection<CommentTree> SortNew(ObservableCollection<CommentTree> list)
        {
            var sortedList = new List<CommentTree>(list);

            sortedList.Sort((x, y) =>
            {
                if (x.Comment.Date > y.Comment.Date)
                    return -1;
                else if (x.Comment.Date < y.Comment.Date)
                    return 1;
                else
                    return 0;
            });

            foreach (var commentTree in sortedList)
            {
                commentTree.Children = SortNew(commentTree.Children);
            }

            return new ObservableCollection<CommentTree>(sortedList);
        }

        /// <summary>
        /// Recursively sorts the list of <see cref="CommentTree"/> from least votes to most votes.
        /// <para>VERY EXPENSIVE OPERATION!</para>
        /// <para>Sort so least votes goes first becaues list view displays the first item on the bottom</para>
        /// </summary>
        /// <param name="list"></param>
        public static ObservableCollection<CommentTree> SortTop(ObservableCollection<CommentTree> list)
        {
            var sortedList = new List<CommentTree>(list);

            sortedList.Sort((x, y) =>
            {
                if (x.Comment.TotalVotes > y.Comment.TotalVotes)
                    return -1;
                else if (x.Comment.TotalVotes < y.Comment.TotalVotes)
                    return 1;
                else
                    return 0;
            });

            foreach (var commentTree in sortedList)
            {
                commentTree.Children = SortNew(commentTree.Children);
            }

            return new ObservableCollection<CommentTree>(sortedList);
        }

        /// <summary>
        /// Recursively count the number of comments in a list of <see cref="CommentTree"/>.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int Count(ObservableCollection<CommentTree> list)
        {
            int counter = 0;

            foreach (var c in list)
            {
                counter += Count(c);
            }

            return counter;
        }

        /// <summary>
        /// Recursively count the number of comments in a <see cref="CommentTree"/>.
        /// </summary>
        /// <param name="tree"></param>
        /// <returns></returns>
        public static int Count(CommentTree tree)
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
    }
}
