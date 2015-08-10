using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private VoatApi voatApi;

        public MainPage()
        {
            this.InitializeComponent();
            voatApi = new VoatApi("ZbDlC73ndD6TB84WQmKvMA==", "https", "fakevout.azurewebsites.net", "api/v1/", "https://fakevout.azurewebsites.net/api/token");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var response = await voatApi.GetSubmissionList("Playground", null);

            if (response != null)
            {
                var items = response.data;
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

            if (submission != null)
            {
                if (submission.Type == 2)
                {
                    SubmissionContentPresenter.ContentTemplate = Resources["SubmissionWebViewTemplate"] as DataTemplate;

                    Debug.WriteLine("SubmissionWebViewTemplate");
                }
                else
                {
                    SubmissionContentPresenter.ContentTemplate = Resources["SubmissionContentTemplate"] as DataTemplate;

                    Debug.WriteLine("SubmissionWebViewTemplate");
                }
            }
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
            
            //    if (sender != null && sender is ContentPresenter)
            //    {
            //        var presenter = sender as ContentPresenter;
            //        var newValue = args.NewValue;

            //        if (newValue != null && newValue is ApiSubmission)
            //        {
            //            var submission = args.NewValue as ApiSubmission;

            //            if (submission != null)
            //            {
            //                if (submission.Type == 2)
            //                {
            //                    presenter.ContentTemplate = Resources["SubmissionWebViewTemplate"] as DataTemplate;

            //                    Debug.WriteLine("SubmissionWebViewTemplate");
            //                }
            //                else
            //                {
            //                    presenter.ContentTemplate = Resources["SubmissionContentTemplate"] as DataTemplate;

            //                    Debug.WriteLine("SubmissionWebViewTemplate");
            //                }
            //            }

            //            Debug.WriteLine(submission.Content);
            //        }
            //    }
        }
    }
}
