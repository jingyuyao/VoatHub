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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as SubmissionLinkVM;
        }

        private void SubmissionUpVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpVote();
        }

        private void SubmissionDownVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DownVote();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            SubmissionWebView.Refresh(); // Neat!
        }

        private void ShowComments_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubmissionCommentsPage), new SubmissionCommentsVM(ViewModel as SubmissionVM));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

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
    }
}
