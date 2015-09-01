using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AccountPage : Page
    {
        private Frame mainFrame;
        private AccountPageVM ViewModel;

        public AccountPage()
        {
            this.InitializeComponent();
            ViewModel = new AccountPageVM();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mainFrame = e.Parameter as Frame;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.VOAT_API.Logout();
            mainFrame.Navigate(typeof(LoginPage));
            mainFrame.BackStack.Clear();
        }
    }
}
