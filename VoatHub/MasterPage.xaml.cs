using System;
using System.Collections.Generic;
using System.Diagnostics;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using VoatHub.Ui;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MasterPage : Page
    {
        private VoatApi VOAT_API = App.VOAT_API;
        private MasterPageVM ViewModel;

        // Misc
        private SolidColorBrush RED_BRUSH = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 0, 0));

        public MasterPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as MasterPageVM;
        }

        #region NewSubmission
        private void NewSubmission_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsNewSubmissionPopupOpen = true;
        }

        /// <summary>
        /// TODO: Better validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewLink_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewLink();
        }

        private void NewDiscussion_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewDiscussion();
        }
        #endregion

        #region AppBar
        private void SortSubmissions_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as MenuFlyoutItem;

            VOAT_API.SubmissionSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.Sort = item.Text; // Need this since we are not refreshing the page

            ViewModel.Refresh();
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Refresh();
        }
        #endregion

        #region SubmissionList
        /// <summary>
        /// Changes which DataTempalte the ContentPresenter uses based on the type of the submission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmissionList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var vm = e.ClickedItem as SubmissionVM;

            if (vm.Submission.Type == ApiSubmissionType.Link)
                ViewModel.DetailFrame.Navigate(typeof(SubmissionLinkPage), new SubmissionLinkVM(vm));
            else
                ViewModel.DetailFrame.Navigate(typeof(SubmissionCommentsPage), new SubmissionCommentsVM(vm));

            ViewModel.DetailFrame.BackStack.Clear();
        }

        private void SubmissionListCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var vm = button.DataContext as SubmissionVM;

            ViewModel.DetailFrame.Navigate(typeof(SubmissionCommentsPage), new SubmissionCommentsVM(vm));
            ViewModel.DetailFrame.BackStack.Clear();
        }
        #endregion

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
    }
}
