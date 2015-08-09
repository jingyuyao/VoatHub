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

        private void SubmissionContentPresenter_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Debug.WriteLine(sender, "Data context changed");

            if (sender != null && sender is ContentPresenter)
            {
                var presenter = sender as ContentPresenter;
                var content = presenter.Content;
                if (content != null && content is ApiSubmission)
                {
                    var submission = content as ApiSubmission;

                    if (submission != null)
                    {
                        if (submission.Type == 2)
                        {
                            presenter.ContentTemplate = Resources["SubmissionWebViewTemplate"] as DataTemplate;

                            Debug.WriteLine("SubmissionWebViewTemplate");
                        }
                        else
                        {
                            presenter.ContentTemplate = Resources["SubmissionContentTemplate"] as DataTemplate;

                            Debug.WriteLine("SubmissionWebViewTemplate");
                        }
                    }

                    Debug.WriteLine(submission.Content);
                }
            }
        }
    }
}
