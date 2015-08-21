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
        // Xaml resources
        private Flyout notFoundFlyout;

        // Page view model
        public MainPageViewModel ViewModel { get; set; }
        
        // Api
        private VoatApi voatApi;
        private SearchOptions submissionSearchOptions;
        private SearchOptions commentSearchOptions;

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
            ViewModel = new MainPageViewModel();

            notFoundFlyout = Resources["NotFoundFlyout"] as Flyout;
            // TODO: Load from settings
            submissionSearchOptions = new SearchOptions();
            commentSearchOptions = new SearchOptions();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            voatApi = e.Parameter as VoatApi;
            
            setCurrentSubverse("_front");

            // TODO: We really need a queue type of thing for the API calls.
            var subscriptions = await voatApi.UserSubscriptions(voatApi.UserName);
            if (subscriptions.Success)
                ViewModel.User.Subscriptions.List = subscriptions.Data;
            ViewModel.User.Subscriptions.Loading = false;

            //var userInfo = await voatApi.UserInfo(voatApi.UserName);
            //if (userInfo.Success)
            //    ViewModel.User.UserInfo = userInfo.Data;
        }

        #region EventHandlers

        #region SplitView

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
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
                    setCurrentSubverse("_front");
                    break;

                case "All":
                    setCurrentSubverse("_all");
                    break;
            }
        }

        private void SubscriptionsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            RootSplitView.IsPaneOpen = false;

            var subscription = e.ClickedItem as ApiSubscription;
            setCurrentSubverse(subscription.Name);
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

            setContentPresenterToSubmission(submission, false);
        }

        private void MasterCommandBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton button = e.OriginalSource as AppBarButton;

            Debug.WriteLine(button, "MasterBar");

            switch (button.Tag.ToString())
            {
                case "refresh":
                    refreshCurrentSubverse();
                    break;
            }
        }

        private void SortSubmissions_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource, "SortItem");
            var item = e.OriginalSource as MenuFlyoutItem;

            submissionSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.SubmissionSort = item.Text;

            refreshCurrentSubverse();
        }

        private void MasterSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            RootSplitView.IsPaneOpen = false;

            var query = args.ChosenSuggestion != null ? args.ChosenSuggestion.ToString() : args.QueryText;

            setCurrentSubverse(query);
            
            sender.Text = "";
        }

        private void SubmissionCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var submission = button.Tag as ApiSubmission;

            // For the initial state where no submission was selected and user presses the comment icon in DetailCommandBar
            if (submission == null) return;

            setContentPresenterToSubmission(submission, true);
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
        private async void NewLink_Click(object sender, RoutedEventArgs e)
        {
            if (!canSubmitToSubverse())
            {
                NewSubmissionPopupErrorBlock.Text = "Cannot submit to subscription sets.";
            }

            if (validateNotEmpty(LinkTitle, LinkUrl) && validatePostTitle(LinkTitle) && validateLinkUrl(LinkUrl))
            {
                toggleProgressRing(NewSubmissionPopupProgressRing);

                var submission = new UserSubmission
                {
                    Title = LinkTitle.Text,
                    Url = LinkUrl.Text
                };
                var r = await voatApi.PostSubmission(ViewModel.MasterColumn.CurrentSubverse, submission);

                if (r.Success)
                {
                    setContentPresenterToSubmission(r.Data, false);
                    NewSubmissionPopup.IsOpen = false;
                    NewSubmissionPopupErrorBlock.Text = "";
                    LinkTitle.Text = "";
                    LinkUrl.Text = "";
                }

                toggleProgressRing(NewSubmissionPopupProgressRing);
            }
        }

        private async void NewDiscussion_Click(object sender, RoutedEventArgs e)
        {
            if (!canSubmitToSubverse())
            {
                NewSubmissionPopupErrorBlock.Text = "Cannot submit to subscription sets.";
            }

            // TODO: Eh, API says content is optional but it actually isn't. wtf
            if (validateNotEmpty(DiscussionTitle, DiscussionContent) && validatePostTitle(DiscussionTitle))
            {
                toggleProgressRing(NewSubmissionPopupProgressRing);

                var submission = new UserSubmission
                {
                    Title = DiscussionTitle.Text,
                    Content = DiscussionContent.Text
                };
                var r = await voatApi.PostSubmission(ViewModel.MasterColumn.CurrentSubverse, submission);

                if (r.Success)
                {
                    setContentPresenterToSubmission(r.Data, false);
                    NewSubmissionPopup.IsOpen = false;
                    NewSubmissionPopupErrorBlock.Text = "";
                    DiscussionTitle.Text = "";
                    DiscussionContent.Text = "";
                }

                toggleProgressRing(NewSubmissionPopupProgressRing);
            }
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
                    refreshCurrentContentPresenter();
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

            commentSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.CommentSort = item.Text;

            refreshCurrentContentPresenter();
        }

        /// <summary>
        /// Fixes WebView size not fit to grid issue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailContentViewer_SizeChanged(object sender, SizeChangedEventArgs e)
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
                submissionViewModel.CommentTree.Add(ct);
            }
        }

        #endregion DetailColumn

        #endregion EventHandler

        #region Helpers

        private async void setCurrentSubverse(string subverse)
        {
            ViewModel.MasterColumn.CurrentlySubscribed = isSubscribed(subverse);

            ViewModel.MasterColumn.CurrentSubverse = subverse;
            ViewModel.MasterColumn.CurrentSubmissions = new LoadingList<ApiSubmission>();
            
            var response = await voatApi.GetSubmissionList(subverse, submissionSearchOptions);
            if (response != null && response.Success)
            {
                ViewModel.MasterColumn.CurrentSubmissions.List = response.Data;
            }

            ViewModel.MasterColumn.CurrentSubmissions.Loading = false;
        }

        private bool isSubscribed(string subverse)
        {
            if (subverse == "_front" || subverse == "_all")
            {
                return true;
            }
            else if (ViewModel.User.Subscriptions != null)
            {
                foreach (var sub in ViewModel.User.Subscriptions.List)
                {
                    if (string.Equals(subverse, sub.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void refreshCurrentSubverse()
        {
            setCurrentSubverse(ViewModel.MasterColumn.CurrentSubverse);
        }

        private async void setContentPresenterToSubmission(ApiSubmission submission, bool forceShowComments)
        {
            // Clears the webview
            DetailWebView.Navigate(new Uri("about:blank"));

            var submissionViewModel = new SubmissionViewModel();
            submissionViewModel.Submission = submission;
            
            if (submission.Type == ApiSubmissionType.Self || forceShowComments)
            {
                submissionViewModel.ShowComments = true;
                submissionViewModel.LoadingComments = true;
                ViewModel.CurrentSubmission = submissionViewModel;

                var response = await voatApi.GetCommentList(submission.Subverse, submission.ID, commentSearchOptions);

                // If the current submission changes, then we released control over the loading icon.
                if (ViewModel.CurrentSubmission.Submission.ID == submission.ID)
                {
                    if (response.Success)
                    {
                        var commentTreeList = CommentTree.FromApiCommentList(response.Data, null);
                        var sortedList = commentTreeSorter(commentTreeList);
                        submissionViewModel.CommentTree = sortedList;
                    }

                    submissionViewModel.LoadingComments = false;
                }
            }
            else
            {
                ViewModel.CurrentSubmission = submissionViewModel;

                Uri uri;
                bool success = Uri.TryCreate(submission.Url, UriKind.Absolute, out uri);

                if (success)
                {
                    DetailWebView.Navigate(uri);
                }
            }
        }

        private void refreshCurrentContentPresenter()
        {
            if (ViewModel.CurrentSubmission != null && ViewModel.CurrentSubmission.Submission != null)
                setContentPresenterToSubmission(ViewModel.CurrentSubmission.Submission, ViewModel.CurrentSubmission.ShowComments);
        }

        /// <summary>
        /// Sorts the commentTreeList based on <see cref="commentSearchOptions"/>.
        /// </summary>
        /// <param name="commentTree"></param>
        private ObservableCollection<CommentTree> commentTreeSorter(ObservableCollection<CommentTree> commentTreeList)
        {
            switch (commentSearchOptions.sort)
            {
                case SortAlgorithm.New:
                    return CommentTree.SortNew(commentTreeList);
                case SortAlgorithm.Top:
                    return CommentTree.SortTop(commentTreeList);
            }

            return commentTreeList;;
        }

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
            var currentSub = ViewModel.MasterColumn.CurrentSubverse;
            return currentSub != "_front" && currentSub != "_all";
        }

        private bool canSubmitToSubverse()
        {
            var current = ViewModel.MasterColumn.CurrentSubverse;
            if (current == "_front" || current == "_all")
                return false;
            else if (ViewModel.User.Subscriptions != null)
            {
                foreach (var sub in ViewModel.User.Subscriptions.List)
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

        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        #endregion Helpers

    }
}
