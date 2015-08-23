using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;
using VoatHub.Ui;
using System.Collections.ObjectModel;

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// <para>Contain mostly event handlers.</para>
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Page view model
        public MainPageVM ViewModel { get; set; }
        
        // Api
        private VoatApi voatApi;

        // SplitView
        public Rect TogglePaneButtonRect
        {
            get;
            private set;
        }

        /// <summary>
        /// An event to notify listeners when the hamburger button may occlude other content in the app.
        /// The custom "PageHeader" user control is using this.
        /// </summary>
        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

        // Misc
        private SolidColorBrush RED_BRUSH = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));

        public MainPage()
        { 
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            voatApi = e.Parameter as VoatApi;
            ViewModel = new MainPageVM(voatApi);
        }

        #region SplitView

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadSubscriptions();
        }

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_UnChecked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            if (handler != null)
            {
                // handler(this, this.TogglePaneButtonRect);
                handler.DynamicInvoke(this, this.TogglePaneButtonRect);
            }
        }

        private void NavMenuList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Close the pane
            RootSplitView.IsPaneOpen = false;

            var item = e.ClickedItem as NavMenuItem;
            switch (item.Label)
            {
                case "Front Page":
                    // Can't fail
                    ViewModel.ChangeSubverse("_front");
                    break;

                case "All":
                    ViewModel.ChangeSubverse("_all");
                    break;
            }
        }

        private void SubscriptionsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            RootSplitView.IsPaneOpen = false;

            var subscription = e.ClickedItem as ApiSubscription;
            ViewModel.ChangeSubverse(subscription.Name);
        }

        #endregion SplitView

        #region MasterColumn

        /// <summary>
        /// Changes which DataTempalte the ContentPresenter uses based on the type of the submission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e == null || e.ClickedItem == null || !(e.ClickedItem is ApiSubmission)) return;

            var submission = e.ClickedItem as ApiSubmission;

            ViewModel.CurrentSubmission.ChangeSubmission(submission, false);
        }

        private void MasterCommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton button = e.OriginalSource as AppBarButton;

            Debug.WriteLine(button, "MasterBar");

            switch (button.Tag.ToString())
            {
                case "refresh":
                    ViewModel.RefreshCurrentSubverse();
                    break;
            }
        }

        private void SortSubmissions_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource, "SortItem");
            var item = e.OriginalSource as MenuFlyoutItem;

            voatApi.SubmissionSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.SubmissionSort = item.Text;

            ViewModel.RefreshCurrentSubverse();
        }

        private void MasterSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            RootSplitView.IsPaneOpen = false;

            var query = args.ChosenSuggestion != null ? args.ChosenSuggestion.ToString() : args.QueryText;

            ViewModel.ChangeSubverse(query);
            
            sender.Text = "";
        }

        private void SubmissionCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var submission = button.Tag as ApiSubmission;

            // For the initial state where no submission was selected and user presses the comment icon in DetailCommandBar
            if (submission == null) return;

            ViewModel.CurrentSubmission.ChangeSubmission(submission, true);
        }

        private void SubscribeButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleButton = e.OriginalSource as AppBarToggleButton;

            // TODO: This is the part where we subscribe and unsubscribe the subverse.
            // Currently the feature is not available in the v1 api as of 8/19/15
            //if (toggleButton.IsChecked == true)
            //{

            //}
        }

        /// <summary>
        /// Assumes the button has the popup in its tag property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var popup = button.Tag as Popup;
            popup.IsOpen = false;
        }

        private void NewPost_Click(object sender, RoutedEventArgs e)
        {
            NewSubmissionPopup.IsOpen = true;
        }

        /// <summary>
        /// TODO: Better validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLink_Click(object sender, RoutedEventArgs e)
        {
            if (!canSubmitToSubverse())
            {
                NewSubmissionPopupErrorBlock.Text = "Cannot submit to subscription sets.";
            }

            if (validateNotEmpty(LinkTitle, LinkUrl) && validatePostTitle(LinkTitle) && validateLinkUrl(LinkUrl))
            {
                var submission = new UserSubmission
                {
                    Title = LinkTitle.Text,
                    Url = LinkUrl.Text
                };

                postSubmission(submission);
            }
        }

        private void NewDiscussion_Click(object sender, RoutedEventArgs e)
        {
            if (!canSubmitToSubverse())
            {
                NewSubmissionPopupErrorBlock.Text = "Cannot submit to subscription sets.";
            }

            // TODO: Eh, API says content is optional but it actually isn't. wtf
            if (validateNotEmpty(DiscussionTitle, DiscussionContent) && validatePostTitle(DiscussionTitle))
            {
                var submission = new UserSubmission
                {
                    Title = DiscussionTitle.Text,
                    Content = DiscussionContent.Text
                };

                postSubmission(submission);
            }
        }

        /// <summary>
        /// Helper method to post new submission and change current submission if post success
        /// </summary>
        /// <param name="submission"></param>
        private async void postSubmission(UserSubmission submission)
        {
            toggleProgressRing(NewSubmissionPopupProgressRing);

            var r = await voatApi.PostSubmission(ViewModel.CurrentSubverse, submission);

            if (r.Success)
            {
                ViewModel.CurrentSubmission.ChangeSubmission(r.Data, false);
                NewSubmissionPopup.IsOpen = false;
                NewSubmissionPopupErrorBlock.Text = "";
                DiscussionTitle.Text = "";
                DiscussionContent.Text = "";
            }

            toggleProgressRing(NewSubmissionPopupProgressRing);
        }

        #endregion MasterColumn

        #region DetailColumn

        /// <summary>
        /// This is fired AFTER DataTemplate is set for the content. So it is not useful to update the data template since it will
        /// not affect the current selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DetailContentPresenter_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Debug.WriteLine(args.NewValue, "Data context change");
        }
        
        private void DetailCommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton button = e.OriginalSource as AppBarButton;

            Debug.WriteLine(button, "DetailBar");

            switch (button.Tag.ToString())
            {
                case "refresh":
                    ViewModel.CurrentSubmission.Refresh();
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

            voatApi.CommentSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            
            ViewModel.CommentSort = item.Text;

            ViewModel.CurrentSubmission.Refresh();
        }

        /// <summary>
        /// Fixes WebView size not fit to grid issue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailTitleRow_SizeChanged(object sender, SizeChangedEventArgs e)
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
            var r = await voatApi.PostCommentReply(comment.Subverse, (int)comment.SubmissionID, comment.ID, value);

            if (r.Success)
            {
                var ct = new CommentTree(r.Data);
                commentTree.Children.Add(ct);
            }
        }

        private void OpenSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentSubmission.ReplyOpen = true;
        }

        private void CloseSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CurrentSubmission.ReplyOpen = false;
        }

        private async void SendSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var submissionViewModel = ViewModel.CurrentSubmission;
            var submission = submissionViewModel.Submission;

            submissionViewModel.ReplyOpen = false;

            var value = new UserValue { Value = submissionViewModel.ReplyText };
            var r = await voatApi.PostComment(submission.Subverse, submission.ID, value);

            if (r.Success)
            {
                var ct = new CommentTree(r.Data);
                submissionViewModel.CommentList.List.Add(ct);
            }
        }

        #endregion DetailColumn
        
        #region Helpers

        private async void commentVotingHelper(Button button, int vote)
        {
            var commentTree = button.DataContext as CommentTree;
            var comment = commentTree.Comment;
            var result = await voatApi.PostVoteRevokeOnRevote("comment", comment.ID, vote, true);
            Debug.WriteLine(result.Data);
        }

        private async void submissionVotingHelper(Button button, int vote)
        {
            var submission = button.Tag as ApiSubmission;
            var result = await voatApi.PostVoteRevokeOnRevote("submission", submission.ID, vote, true);
        }

        private void toggleProgressRing(ProgressRing ring)
        {
            ring.Visibility = ring.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            ring.IsActive = ring.IsActive ? false : true;
        }

        /// <summary>
        /// Go through every box and highlight it red if it's empty.
        /// </summary>
        /// <param name="boxes"></param>
        /// <returns>Whether all box pass the check.</returns>
        private bool validateNotEmpty(params TextBox[] boxes)
        {
            bool pass = true;

            foreach (var box in boxes)
            {
                if (box.Text == "" || box.Text == null)
                {
                    invalidateBox(box);
                    pass = false;
                }
            }
            return pass;
        }

        private bool validatePostTitle(TextBox box)
        {
            if (box.Text.Length < 5)
            {
                invalidateBox(box);
                return false;
            }

            return true;
        }

        private bool validateLinkUrl(TextBox box)
        {
            Uri uri;
            bool success = Uri.TryCreate(box.Text, UriKind.Absolute, out uri);
            if (!success) invalidateBox(box);
            return success;
        }

        private void invalidateBox(TextBox box)
        {
            box.BorderBrush = RED_BRUSH;
        }

        private bool inValidSubverse()
        {
            var currentSub = ViewModel.CurrentSubverse;
            return currentSub != "_front" && currentSub != "_all";
        }

        private bool canSubmitToSubverse()
        {
            var current = ViewModel.CurrentSubverse;
            if (current == "_front" || current == "_all")
                return false;
            else if (ViewModel.Subscriptions != null)
            {
                foreach (var sub in ViewModel.Subscriptions.List)
                {
                    if (string.Equals(current, sub.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return sub.Type == SubscriptionType.Subverse;
                    }
                }
            }

            // current subverse is not one of the special ones and its not in the subscription list that contains all the sets.
            return true;
        }
        #endregion Helpers

        private void PrintDataContext_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            Debug.WriteLine(button.DataContext);
        }
    }
}
