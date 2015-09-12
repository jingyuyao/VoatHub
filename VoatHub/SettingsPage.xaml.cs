using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class SettingsPage : Page
    {
        private Flyout restartNeededFlyout;

        public SettingsPage()
        {
            this.InitializeComponent();

            restartNeededFlyout = Resources["RestartNeededFlyout"] as Flyout;

            var theme = App.Current.RequestedTheme;
            ThemeComboBox.SelectedItem = theme == ApplicationTheme.Dark ? ThemeDarkItem : ThemeLightItem;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;

            if (combobox.SelectedItem == null) return;

            var selected = combobox.SelectedItem as ComboBoxItem;
            var settings = ApplicationData.Current.RoamingSettings;

            if (selected == ThemeDarkItem)
                App.ChangeThemeSetting(ApplicationTheme.Dark);
            else if (selected == ThemeLightItem)
                App.ChangeThemeSetting(ApplicationTheme.Light);

            // The try/catch prevents error occured when the initial loading selected changed 
            // trigger that happens before the visual tree is shown.
            try
            {
                restartNeededFlyout.ShowAt(combobox);
            }
            catch (ArgumentException) { }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }
    }
}
