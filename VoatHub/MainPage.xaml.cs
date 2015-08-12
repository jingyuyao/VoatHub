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

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// <para>Contain mostly event handlers.</para>
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Xaml resources
        private DataTemplate linkSubmissionTemplate;
        private DataTemplate commentSubmissionTemplate;
        private Flyout notFoundFlyout;

        // Page view model
        public MainPageViewModel ViewModel { get; set; }
        
        // Api
        private VoatApi voatApi;
        private SearchOptions submissionSearchOptions;

        public MainPage()
        { 
            this.InitializeComponent();

            linkSubmissionTemplate = Resources["LinkSubmissionTemplate"] as DataTemplate;
            commentSubmissionTemplate = Resources["CommentSubmissionTemplate"] as DataTemplate;
            notFoundFlyout = Resources["NotFoundFlyout"] as Flyout;

            ViewModel = new MainPageViewModel();

            voatApi = new VoatApi("ZbDlC73ndD6TB84WQmKvMA==", "https", "fakevout.azurewebsites.net", "api/v1/", "https://fakevout.azurewebsites.net/api/token");
            submissionSearchOptions = new SearchOptions();
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await setCurrentSubverse("_front");
        }

        // Event handlers
        
        /// <summary>
        /// Changes which DataTempalte the ContentPresenter uses based on the type of the submission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e == null || e.ClickedItem == null || !(e.ClickedItem is ApiSubmission)) return;

            var submission = e.ClickedItem as ApiSubmission;

            setContentPresenterToSubmission(submission);
        }
        
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

        private void SortSubmissions_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource, "SortItem");
            var item = e.OriginalSource as MenuFlyoutItem;

            switch (item.Tag.ToString())
            {
                case "hot":
                    submissionSearchOptions.sort = SortAlgorithm.Hot;
                    break;

                case "new":
                    submissionSearchOptions.sort = SortAlgorithm.New;
                    break;

                case "top":
                    submissionSearchOptions.sort = SortAlgorithm.Top;
                    break;
            }

            refreshCurrentSubverse();
        }

        private async void MasterSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var query = args.ChosenSuggestion != null ? args.ChosenSuggestion.ToString() : args.QueryText;

            bool success = await setCurrentSubverse(query);

            if (!success)
                notFoundFlyout.ShowAt(sender);
        }

        // Helper methods

        private async Task<bool> setCurrentSubverse(string subverse)
        {
            toggleMasterListState();

            var response = await voatApi.GetSubmissionList(subverse, submissionSearchOptions);
            if (response != null && response.Success)
            {
                ViewModel.CurrentSubverse = subverse;
                MasterListView.ItemsSource = response.Data;

                toggleMasterListState();
                return true;
            }

            toggleMasterListState();
            return false;
        }

        private void toggleMasterListState()
        {
            MasterProgressRing.IsActive = !MasterProgressRing.IsActive;
            MasterProgressRing.Visibility = MasterProgressRing.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            MasterListView.Visibility = MasterListView.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void refreshCurrentSubverse()
        {
            await setCurrentSubverse(ViewModel.CurrentSubverse);
        }

        private void setContentPresenterToSubmission(ApiSubmission submission)
        {
            ViewModel.CurrentPresentedSubmission = submission;

            if (submission.Type == ApiSubmissionType.Link)
            {
                setContentPresenterToLink(submission);
            }
            else
            {
                setContentPresenterToComment(submission);
            }
        }

        /// <summary>
        /// Set <see cref="SubmissionContentPresenter"/> to display a web link.
        /// </summary>
        /// <param name="submission">Can only be link post.</param>
        private void setContentPresenterToLink(ApiSubmission submission)
        {
            Debug.WriteLine("setContentPresenterToLink");
            DetailContentPresenter.Content = submission;
            DetailContentPresenter.ContentTemplate = linkSubmissionTemplate;
        }

        /// <summary>
        /// Set <see cref="SubmissionContentPresenter"/> to display a comment thread.
        /// <para>The Content is set twice. Once initialy with the submission content and once
        /// when the comments are retrieved from the API.</para>
        /// </summary>
        /// <param name="submission">Can be both link and self post</param>
        private async void setContentPresenterToComment(ApiSubmission submission)
        {
            Debug.WriteLine("setContentPresenterToComment");
            var submissionWithComment = new CommentSubmission();
            submissionWithComment.Submission = submission;
            submissionWithComment.LoadingComments = true;

            // Set content before comments are retrieved to show things faster.
            DetailContentPresenter.Content = submissionWithComment;
            DetailContentPresenter.ContentTemplate = commentSubmissionTemplate;

            var response = await voatApi.GetCommentList(submission.Subverse, submission.ID, null);

            if (response.Success)
            {
                submissionWithComment.Comments = response.Data;
            }

            submissionWithComment.LoadingComments = false;

            // Set the content again to show comments.
            // NOTE: changing a property of the content does not redraw the content. We must
            // Set the content property again to notify the event listeners of the change.
            // NOTE2: Since seconds can go by before we receive response from server we have to
            // make sure the content in the presenter is still the same submission we are retrieving
            // comments for before we refresh it.
            if (ViewModel.CurrentPresentedSubmission.ID == submission.ID)
                DetailContentPresenter.Content = submissionWithComment;
        }

        private void refreshCurrentContentPresenter()
        {
            setContentPresenterToSubmission(ViewModel.CurrentPresentedSubmission);
        }
    }
}
