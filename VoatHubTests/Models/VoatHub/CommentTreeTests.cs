using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;

namespace VoatHubTests.Models.VoatHub
{
    [TestClass]
    public class CommentTreeTests
    {
        [TestMethod]
        public void Creation()
        {
            var comments = new List<ApiComment>();
            comments.Add(new ApiComment
            {
                ID = 1,
                Level = 0,
            });
            comments.Add(new ApiComment
            {
                ID = 2,
                Level = 1,
                ParentID = 1
            });
            comments.Add(new ApiComment
            {
                ID = 3,
                Level = 1,
                ParentID = 1
            });
            comments.Add(new ApiComment
            {
                ID = 4,
                Level = 2,
                ParentID = 2
            });
            comments.Add(new ApiComment
            {
                ID = 5,
                Level = 0
            });

            //var commentTreeList = CommentVM.FromApiCommentList(comments, null);
            //Assert.AreEqual(2, commentTreeList.Count);
            //Assert.AreEqual(5, CommentVM.Count(commentTreeList));

            //var first = commentTreeList.Find(i => i.Comment.ID == 1);
            //Assert.AreEqual(2, first.Children.Count);

            //var second = first.Children.Find(i => i.Comment.ID == 2);
            //Assert.AreEqual(1, second.Children.Count);

            //var fifth = commentTreeList.Find(i => i.Comment.ID == 5);
            //Assert.AreEqual(0, fifth.Children.Count);

        }
    }
}
