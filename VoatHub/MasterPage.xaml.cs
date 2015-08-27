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

        #region Helpers
        /// <summary>
        /// Helper method to post new submission and change current submission if post success
        /// </summary>
        /// <param name="submission"></param>
        private async void postSubmission(UserSubmission submission)
        {
            NewSubmissionPopupProgressRing.Toggle();

            var r = await VOAT_API.PostSubmission(ViewModel.Subverse, submission);

            if (r.Success)
            {
                ViewModel.DetailFrame.Navigate(typeof(DetailPage), new DetailPageVM(r.Data, false));
                NewSubmissionPopup.IsOpen = false;
                NewSubmissionPopupErrorBlock.Text = "";
                DiscussionTitle.Text = "";
                DiscussionContent.Text = "";
            }

            NewSubmissionPopupProgressRing.Toggle();
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
        #endregion

        /// <summary>
        /// Changes which DataTempalte the ContentPresenter uses based on the type of the submission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var vm = e.ClickedItem as SubmissionVM;

            ViewModel.DetailFrame.Navigate(typeof(DetailPage), new DetailPageVM(vm.Submission, false));
        }

        private void SortSubmissions_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource, "SortItem");
            var item = e.OriginalSource as MenuFlyoutItem;

            VOAT_API.SubmissionSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.Sort = item.Text;

            ViewModel.Refresh();
        }

        private void SubmissionCommentsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var vm = button.DataContext as SubmissionVM;

            ViewModel.DetailFrame.Navigate(typeof(DetailPage), new DetailPageVM(vm.Submission, true));
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
            if (!ViewModel.CanPost)
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
            if (!ViewModel.CanPost)
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Refresh();
        }
    }
}
