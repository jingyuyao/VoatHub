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

        public MainPage()
        { 
            this.InitializeComponent();
            notFoundFlyout = Resources["NotFoundFlyout"] as Flyout;
            //DetailWebView.NavigationCompleted += (sender, args) => sender.ResizeToContent();

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

            setContentPresenterToSubmission(submission, false);
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
            var submissionViewModel = new SubmissionViewModel();
            submissionViewModel.Submission = submission;
            
            if (submission.Type == ApiSubmissionType.Self || forceShowComments)
            {
                submissionViewModel.ShowComments = true;
                submissionViewModel.LoadingComments = true;
                ViewModel.CurrentSubmission = submissionViewModel;

                var response = await voatApi.GetCommentList(submission.Subverse, submission.ID, null);

                if (response.Success)
                {
                    submissionViewModel.CommentTree = CommentTree.FromApiCommentList(response.Data, null);
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

        private void DetailContentViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DetailWebView.Height = DetailContentViewer.ActualHeight - DetailTitleRow.ActualHeight;
            DetailWebView.Width = DetailInnerColumn.ActualWidth;
        }
    }
}
