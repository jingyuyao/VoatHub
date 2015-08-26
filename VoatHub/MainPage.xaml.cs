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
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub;
using VoatHub.Ui;
using System.Collections.ObjectModel;

namespace VoatHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// <para>Contain mostly event handlers.</para>
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Page view model
        public MainPageVM ViewModel { get; set; }
        
        // Api
        private VoatApi VOAT_API = App.VOAT_API;

        // SplitView
        public Rect TogglePaneButtonRect
        {
            get;
            private set;
        }

        /// <summary>
        /// An event to notify listeners when the hamburger button may occlude other content in the app.
        /// The custom "PageHeader" user control is using this.
        /// </summary>
        public event TypedEventHandler<MainPage, Rect> TogglePaneButtonRectChanged;

        public MainPage()
        { 
            this.InitializeComponent();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = new MainPageVM();
            changeSubverse("_front");
        }

        #region SplitView

        private void TogglePaneButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadSubscriptions();
        }

        /// <summary>
        /// Callback when the SplitView's Pane is toggled open or close.  When the Pane is not visible
        /// then the floating hamburger may be occluding other content in the app unless it is aware.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TogglePaneButton_UnChecked(object sender, RoutedEventArgs e)
        {
            this.CheckTogglePaneButtonSizeChanged();
        }

        /// <summary>
        /// Check for the conditions where the navigation pane does not occupy the space under the floating
        /// hamburger button and trigger the event.
        /// </summary>
        private void CheckTogglePaneButtonSizeChanged()
        {
            if (this.RootSplitView.DisplayMode == SplitViewDisplayMode.Inline ||
                this.RootSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                var transform = this.TogglePaneButton.TransformToVisual(this);
                var rect = transform.TransformBounds(new Rect(0, 0, this.TogglePaneButton.ActualWidth, this.TogglePaneButton.ActualHeight));
                this.TogglePaneButtonRect = rect;
            }
            else
            {
                this.TogglePaneButtonRect = new Rect();
            }

            var handler = this.TogglePaneButtonRectChanged;
            if (handler != null)
            {
                // handler(this, this.TogglePaneButtonRect);
                handler.DynamicInvoke(this, this.TogglePaneButtonRect);
            }
        }

        private void NavMenuList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Close the pane
            RootSplitView.IsPaneOpen = false;

            var item = e.ClickedItem as NavMenuItem;
            switch (item.Label)
            {
                case "Front Page":
                    // Can't fail
                    changeSubverse("_front");
                    break;

                case "All":
                    changeSubverse("_all");
                    break;
            }
        }

        private void SubscriptionsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            RootSplitView.IsPaneOpen = false;

            var subscription = e.ClickedItem as ApiSubscription;
            changeSubverse(subscription.Name);
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            RootSplitView.IsPaneOpen = false;

            var query = args.ChosenSuggestion != null ? args.ChosenSuggestion.ToString() : args.QueryText;

            changeSubverse(query);

            sender.Text = "";
        }

        #endregion SplitView

        private void changeSubverse(string subverse)
        {
            MasterFrame.Navigate(typeof(MasterPage), new MasterPageVM(subverse, ViewModel.IsSubscribed(subverse), ViewModel.CanSubmit(subverse), DetailFrame));
        }
    }
}
