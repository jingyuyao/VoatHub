using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
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
    public sealed partial class SubmissionCommentsPage : Page
    {
        private VoatApi VOAT_API = App.VOAT_API;
        private SubmissionCommentsVM ViewModel;

        public SubmissionCommentsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as SubmissionCommentsVM;
        }

        #region Misc
        private void PrintDataContext_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            Debug.WriteLine(button.DataContext);
        }
        #endregion

        #region CommentTree
        private void CommentHeaderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentVM = button.DataContext as CommentVM;
            commentVM.ToggleVisibility();
        }

        private void CommentUpVote_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentVM = button.DataContext as CommentVM;
            commentVM.UpVote();
        }

        private void CommentDownVote_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var commentVM = button.DataContext as CommentVM;
            commentVM.DownVote();
        }

        private void CommentReplyButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as HyperlinkButton;
            var vm = button.DataContext as CommentVM;
            vm.ReplyOpen = true;
        }

        private void CloseCommentReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var vm = button.DataContext as CommentVM;
            vm.ReplyOpen = false;
        }

        private async void SendCommentReply_Click(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var vm = button.DataContext as CommentVM;
            var reply = await vm.SendReply();

            var list = ViewModel.Comments.List;
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (reply.Comment.ParentID == list[i].Comment.ID)
                {
                    list.Insert(i + 1, reply);
                    return;
                }
            }
            list.Add(reply);
        }
        #endregion

        #region AppBar
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadComments();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
        }

        /// <summary>
        /// Change the sorting options and refresh the page. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortComments_Click(object sender, RoutedEventArgs e)
        {
            var item = e.OriginalSource as MenuFlyoutItem;
            VOAT_API.CommentSearchOptions.sort = (SortAlgorithm)Enum.Parse(typeof(SortAlgorithm), item.Text);
            ViewModel.CommentSort = item.Text;
            ViewModel.LoadComments();
        }
        #endregion

        #region Submission
        private void SubmissionUpVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UpVote();
        }

        private void SubmissionDownVote_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DownVote();
        }

        private void OpenSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReplyOpen = true;
        }

        private void CloseSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ReplyOpen = false;
        }

        private void SendSubmissionReply_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SendSubmissionReply();
        }
        #endregion
    }
}
