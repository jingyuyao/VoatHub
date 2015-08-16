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

        public MainPage()
        { 
            this.InitializeComponent();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            notFoundFlyout = Resources["NotFoundFlyout"] as Flyout;

            ViewModel = new MainPageViewModel();
            ViewModel.CurrentSubmission = new SubmissionViewModel();

            // TODO: Load from settings
            submissionSearchOptions = new SearchOptions();
            commentSearchOptions = new SearchOptions();

            voatApi = e.Parameter as VoatApi;
            await setCurrentSubverse("_front");
        }

        // Event handlers
        
        // MasterColumnGrid

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

            sortAlgorithmHelper(item.Tag.ToString(), submissionSearchOptions);

            refreshCurrentSubverse();
        }

        private async void MasterSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var query = args.ChosenSuggestion != null ? args.ChosenSuggestion.ToString() : args.QueryText;

            bool success = await setCurrentSubverse(query);

            if (!success)
                notFoundFlyout.ShowAt(sender);
        }

        private void SubmissionCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var submission = button.Tag as ApiSubmission;

            // For the initial state where no submission was selected and user presses the comment icon in DetailCommandBar
            if (submission == null) return;

            setContentPresenterToSubmission(submission, true);
        }

        // DetailColumnGrid

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

            sortAlgorithmHelper(item.Tag.ToString(), commentSearchOptions);

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

        // Helper methods

        private async Task<bool> setCurrentSubverse(string subverse)
        {
            ViewModel.LoadingSubmissions = true;

            var response = await voatApi.GetSubmissionList(subverse, submissionSearchOptions);
            if (response != null && response.Success)
            {
                ViewModel.CurrentSubverse = subverse;
                MasterListView.ItemsSource = response.Data;

                ViewModel.LoadingSubmissions = false;
                return true;
            }

            ViewModel.LoadingSubmissions = false;
            return false;
        }

        private async void refreshCurrentSubverse()
        {
            await setCurrentSubverse(ViewModel.CurrentSubverse);
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

                if (response.Success)
                {
                    var commentTreeList = CommentTree.FromApiCommentList(response.Data, null);
                    commentTreeSorter(commentTreeList);
                    submissionViewModel.CommentTree = commentTreeList;
                }

                submissionViewModel.LoadingComments = false;
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
            setContentPresenterToSubmission(ViewModel.CurrentSubmission.Submission, ViewModel.CurrentSubmission.ShowComments);
        }

        /// <summary>
        /// Changes the sort parameter in options to the corresponding tag.
        /// </summary>
        /// <param name="sortTag"></param>
        /// <param name="options"></param>
        private void sortAlgorithmHelper(string sortTag, SearchOptions options)
        {
            switch (sortTag)
            {
                case "hot":
                    options.sort = SortAlgorithm.Hot;
                    break;

                case "new":
                    options.sort = SortAlgorithm.New;
                    break;

                case "top":
                    options.sort = SortAlgorithm.Top;
                    break;
            }
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
    }
}
