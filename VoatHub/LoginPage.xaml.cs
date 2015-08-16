using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using VoatHub.Api.Client;
using VoatHub.Api.Voat;

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        VoatApi voatApi;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            voatApi = new VoatApi("ZbDlC73ndD6TB84WQmKvMA==", "https", "fakevout.azurewebsites.net", "api/v1/", "https://fakevout.azurewebsites.net/api/token");

            if (voatApi.LoggedIn)
            {
                // Some scheduling bullshit workaround.
                // https://social.msdn.microsoft.com/Forums/en-US/af8f62e6-3c0d-4257-adac-4cecf1e8e23c/onnavigatedto-wont-navigate-to-another-page?forum=winappswithcsharp
                // http://stackoverflow.com/questions/25942567/automatically-navigate-to-another-page
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Frame.Navigate(typeof(MainPage), voatApi);
                });
            }
            else
            {
                LoginPanel.Visibility = Visibility.Visible;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameField.Text;
            var password = PasswordField.Password;

            LoginProgressRing.IsActive = true;

            bool loggedIn = await voatApi.Login(username, password);

            if (loggedIn)
            {
                Frame.Navigate(typeof(MainPage), voatApi);
            }
            else
            {
                PasswordField.Password = "";
                LoginProgressRing.IsActive = false;
                LoginErrorBlock.Text = "Login Error. Please try again.";
                LoginErrorBlock.Visibility = Visibility.Visible;
            }
        }

        private async void PasswordField_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var username = UsernameField.Text;
                var password = PasswordField.Password;

                LoginProgressRing.IsActive = true;

                bool loggedIn = await voatApi.Login(username, password);

                if (loggedIn)
                {
                    Frame.Navigate(typeof(MainPage), voatApi);
                }
                else
                {
                    PasswordField.Password = "";
                    LoginProgressRing.IsActive = false;
                    LoginErrorBlock.Text = "Login Error. Please try again.";
                    LoginErrorBlock.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
