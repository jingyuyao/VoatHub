using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    public class CommentTree
    {
        public CommentTree(ApiComment comment)
        {
            if (comment == null) throw new ArgumentException("Comment cannot be null.");

            Comment = comment;
            Children = new List<CommentTree>();
        }

        /// <summary>
        /// Invariant: never null.
        /// </summary>
        public ApiComment Comment { get; }
        /// <summary>
        /// Invariant: never null.
        /// </summary>
        public List<CommentTree> Children { get; set; }
        public bool HasChildren
        {
            get
            {
                return Children.Count != 0;
            }
        }

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

        private static CommentTree FindParentList(ApiComment target, List<CommentTree> source)
        {
            for (var i = 0; i < source.Count; i++)
            {
                var result = source[i].FindParent(target);
                if (result != null) return result;
            }
            
            return null;
        }

        public static List<CommentTree> FromApiCommentList(List<ApiComment> apiComments, List<CommentTree> source)
        {
            var rootNestedComments = source ?? new List<CommentTree>();

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
    }
}
