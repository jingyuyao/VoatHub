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

        //private void SubscriptionsList_Click(object sender, RoutedEventArgs e)
        //{
        //    SubscriptionsPopup.IsOpen = !SubscriptionsPopup.IsOpen;
        //}

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

        #endregion DetailColumn

        #endregion EventHandler

        #region Helpers

        private async void setCurrentSubverse(string subverse)
        {
            ViewModel.MasterColumn.CurrentSubverse = subverse;
            ViewModel.MasterColumn.CurrentSubmissions = new LoadingList<ApiSubmission>();
            
            var response = await voatApi.GetSubmissionList(subverse, submissionSearchOptions);
            if (response != null && response.Success)
            {
                ViewModel.MasterColumn.CurrentSubmissions.List = response.Data;
            }

            ViewModel.MasterColumn.CurrentSubmissions.Loading = false;
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
                        commentTreeSorter(commentTreeList);
                        submissionViewModel.CommentTree = commentTreeList;
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
        private void commentTreeSorter(List<CommentTree> commentTreeList)
        {
            switch (commentSearchOptions.sort)
            {
                case SortAlgorithm.New:
                    CommentTree.SortNew(commentTreeList);
                    break;
                case SortAlgorithm.Top:
                    CommentTree.SortTop(commentTreeList);
                    break;
            }

            Debug.WriteLine(commentTreeList);
        }

        private async void commentVotingHelper(Button button, int vote)
        {
            var comment = button.Tag as ApiComment;
            var result = await voatApi.PostVoteRevokeOnRevote("comment", comment.ID, vote, true);
            Debug.WriteLine(result.Data);
        }

        private async void submissionVotingHelper(Button button, int vote)
        {
            var submission = button.Tag as ApiSubmission;
            var result = await voatApi.PostVoteRevokeOnRevote("submission", submission.ID, vote, true);
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
        #endregion Helpers
    }
}
