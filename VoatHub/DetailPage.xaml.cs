using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;
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
    /// A self contained page showing the contents of a self post or link post.
    /// </summary>
    public sealed partial class DetailPage : Page
    {
        private VoatApi VOAT_API = App.VOAT_API;
        private DetailPageVM ViewModel;

        public DetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as DetailPageVM;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ViewModel.Uri = DetailPageVM.DEFAULT_URI;
        }

        /// <summary>
        /// Fixes WebView size not fit to grid issue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                DetailWebView.Height = DetailContentViewer.ActualHeight - DetailTitleRow.ActualHeight;
                DetailWebView.Width = DetailInnerColumn.ActualWidth;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SubmissionUpVote_Click(object sender, RoutedEventArgs e)
        {
            submissionVotingHelper(e.OriginalSource as Button, 1);
        }

        private void SubmissionDownVote_Click(object sender, RoutedEventArgs e)
        {
            submissionVotingHelper(e.OriginalSource as Button, -1);
        }

        private void CommentUpVote_Click(object sender, RoutedEventArgs e)
        {
            commentVotingHelper(e.OriginalSource as Button, 1);
        }

        private void CommentDownVote_Click(object sender, RoutedEventArgs e)
        {
            commentVotingHelper(e.OriginalSource as Button, -1);
        }

        private void OpenCommentReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentTree = button.DataContext as CommentTree;
            commentTree.ReplyOpen = true;
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
                var ct = new CommentTree(r.Data);
                commentTree.Children.Add(ct);
            }
        }

        private void OpenSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReplyOpen = true;
        }

        private void CloseSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReplyOpen = false;
        }

        private async void SendSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var submissionViewModel = ViewModel;
            var submission = submissionViewModel.Submission;

            submissionViewModel.ReplyOpen = false;

            var value = new UserValue { Value = submissionViewModel.ReplyText };
            var r = await VOAT_API.PostComment(submission.Subverse, submission.ID, value);

            if (r.Success)
            {
                var ct = new CommentTree(r.Data);
                submissionViewModel.CommentList.List.Add(ct);
            }
        }

        private void DetailCommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton button = e.OriginalSource as AppBarButton;

            Debug.WriteLine(button, "DetailBar");

            switch (button.Tag.ToString())
            {
                case "refresh":
                    ViewModel.Refresh();
                    break;
            }
        }

        /// <summary>
        /// This actually doesn't do shit. Voat returns the same thing no matter what sort options we send it...
        /// <para>We actually have to do the sorting ourselves which kind of makes sense.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortComments_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as MenuFlyoutItem;

            VOAT_API.CommentSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);

            ViewModel.CommentSort = item.Text;

            ViewModel.Refresh();
        }

        private void ShowComments_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DetailPage), new DetailPageVM(ViewModel.Submission, true));
        }

        private async void commentVotingHelper(Button button, int vote)
        {
            var commentTree = button.DataContext as CommentTree;
            var comment = commentTree.Comment;
            var result = await VOAT_API.PostVoteRevokeOnRevote("comment", comment.ID, vote, true);
            Debug.WriteLine(result.Data);
        }

        private async void submissionVotingHelper(Button button, int vote)
        {
            var submission = button.Tag as ApiSubmission;
            var result = await VOAT_API.PostVoteRevokeOnRevote("submission", submission.ID, vote, true);
        }

        private void PrintDataContext_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            Debug.WriteLine(button.DataContext);
        }
    }
}
