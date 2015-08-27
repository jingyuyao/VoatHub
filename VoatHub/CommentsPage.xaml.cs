using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.VoatHub;
using VoatHub.Models.VoatHub.LoadingList;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentsPage : Page
    {
        private VoatApi VOAT_API = App.VOAT_API;
        public CommentsVM ViewModel;

        public CommentsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as CommentsVM;
        }

        #region Helpers

        private async void commentVotingHelper(Button button, int vote)
        {
            var commentTree = button.DataContext as CommentTree;
            var comment = commentTree.Comment;
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", comment.ID, vote, true);
            Debug.WriteLine(result.Data);
        }

        private void PrintDataContext_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            Debug.WriteLine(button.DataContext);
        }
        #endregion

        private void CommentUpVote_Click(object sender, RoutedEventArgs e)
        {
            commentVotingHelper(e.OriginalSource as Button, 1);
        }

        private void CommentDownVote_Click(object sender, RoutedEventArgs e)
        {
            commentVotingHelper(e.OriginalSource as Button, -1);
        }

        private void CloseCommentReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentTree = button.DataContext as CommentTree;
            commentTree.ReplyOpen = false;
        }

        private async void SendCommentReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentTree = button.DataContext as CommentTree;
            var comment = commentTree.Comment;

            commentTree.ReplyOpen = false;

            var value = new UserValue { Value = commentTree.ReplyText };
            var r = await VOAT_API.PostCommentReply(comment.Subverse, (int)comment.SubmissionID, comment.ID, value);

            if (r.Success)
            {
                var newComment = r.Data;
                newComment.Level = comment.Level + 1;  // Fixes api not returning proper level for newly created comments
                commentTree.Children.Add(new CommentTree(newComment));
            }
        }

        private void CommentReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var commentTree = button.DataContext as CommentTree;
            commentTree.ReplyOpen = true;
        }
    }
}
