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
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Xaml resources
        private DataTemplate linkSubmissionTemplate;
        private DataTemplate commentSubmissionTemplate;

        // Page data
        private VoatApi voatApi;

        public MainPage()
        {
            this.InitializeComponent();

            linkSubmissionTemplate = Resources["LinkSubmissionTemplate"] as DataTemplate;
            commentSubmissionTemplate = Resources["CommentSubmissionTemplate"] as DataTemplate;

            voatApi = new VoatApi("ZbDlC73ndD6TB84WQmKvMA==", "https", "fakevout.azurewebsites.net", "api/v1/", "https://fakevout.azurewebsites.net/api/token");
        }
        
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var response = await voatApi.GetSubmissionList("Playground", null);

            if (response != null)
            {
                var items = response.Data;
                SubmissionListView.ItemsSource = items;
            }
        }

        /// <summary>
        /// Sets the data to the UI elements based on the clicked ApiSubmission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void SubmissionListView_ItemClick(object sender, ItemClickEventArgs e)
        //{
        //    if (e == null || e.ClickedItem == null || !(e.ClickedItem is ApiSubmission)) return;

        //    var submission = e.ClickedItem as ApiSubmission;

        //    SubmissionTitleTextBlock.Text = submission.Title ?? "";
        //    if (submission.Type == 2)
        //    {
        //        if (submission.Url != null)
        //        {
        //            Uri uri;
        //            bool uriOk = Uri.TryCreate(submission.Url, UriKind.Absolute, out uri);
        //            if (uriOk)
        //            {
        //                SubmissionWebView.Navigate(uri);
        //                SubmissionWebView.Visibility = Visibility.Visible;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        SubmissionWebView.Visibility = Visibility.Collapsed;
        //    }

        //    if (submission.Content == null) SubmissionContentTextBlock.Visibility = Visibility.Collapsed;
        //    else SubmissionContentTextBlock.Text = submission.Content;

        //    Debug.WriteLine(submission.Content);
        //}

        /// <summary>
        /// Changes which DataTempalte the ContentPresenter uses based on the type of the submission.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmissionListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e == null || e.ClickedItem == null || !(e.ClickedItem is ApiSubmission)) return;

            var submission = e.ClickedItem as ApiSubmission;

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
        /// <param name="submission"></param>
        private void setContentPresenterToLink(ApiSubmission submission)
        {
            SubmissionContentPresenter.Content = submission;
            SubmissionContentPresenter.ContentTemplate = linkSubmissionTemplate;
            Debug.WriteLine("LinkSubmissionTemplate");
        }

        /// <summary>
        /// Set <see cref="SubmissionContentPresenter"/> to display a comment thread.
        /// <para>The Content is set twice. Once initialy with the submission content and once
        /// when the comments are retrieved from the API.</para>
        /// </summary>
        /// <param name="submission"></param>
        private async void setContentPresenterToComment(ApiSubmission submission)
        {
            var submissionWithComment = new CommentSubmission();
            submissionWithComment.Submission = submission;

            // Set content before list to comments is retrieved to show things faster.
            SubmissionContentPresenter.Content = submissionWithComment;
            SubmissionContentPresenter.ContentTemplate = commentSubmissionTemplate;

            var response = await voatApi.GetCommentList(submission.Subverse, submission.ID, null);

            if (response.Success)
            {
                submissionWithComment.Comments = response.Data;
            }

            // Set the content again to show comments.
            // NOTE: changing a property of the content does not redraw the content. We must
            // Set the content property again to notify the event listeners of the change.
            SubmissionContentPresenter.Content = submissionWithComment;
            Debug.WriteLine("SubmissionWebViewTemplate");
        }

        /// <summary>
        /// This is fired AFTER DataTemplate is set for the content. So it is not useful to update the data template since it will
        /// not affect the current selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SubmissionContentPresenter_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Debug.WriteLine(args.NewValue, "Data context change");
        }
    }
}
