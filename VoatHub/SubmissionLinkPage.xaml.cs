using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubmissionLinkPage : Page
    {
        private SubmissionLinkVM ViewModel;

        public SubmissionLinkPage()
        {
            this.InitializeComponent();
        }

        #region Page
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as SubmissionLinkVM;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            SubmissionWebView.Stop();
        }
        #endregion

        #region AppBar
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SubmissionWebView.Refresh();
        }

        /// <summary>
        /// Go to the comment page. Current page is perseved on the stack.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowComments_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubmissionCommentsPage), new SubmissionCommentsVM(ViewModel as SubmissionVM));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
        #endregion

        #region SubmissionHeader
        private void SubmissionUpVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpVote();
        }

        private void SubmissionDownVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DownVote();
        }
        #endregion

        #region WebView
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                SubmissionWebView.Height = WebViewRow.ActualHeight;
                SubmissionWebView.Width = RootColumn.ActualWidth;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
        #endregion
    }
}
