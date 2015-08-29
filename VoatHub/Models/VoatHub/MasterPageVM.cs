using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub.LoadingList;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VoatHub.Models.VoatHub
{
    public class MasterPageVM : BindableBase
    {
        private static readonly VoatApi VOAT_API = App.VOAT_API;

        /// <summary>
        /// Allows MasterPage to control DetailFrame
        /// </summary>
        public Frame DetailFrame { get; }

        public MasterPageVM(string subverse, bool isSubscribed, bool canPost, Frame detailFrame)
        {
            Subverse = subverse;
            Sort = VOAT_API.SubmissionSearchOptions.sort.ToString();
            CanPost = canPost;

            IsSubscribed = isSubscribed;
            IsNewSubmissionPopupOpen = false;
            NewSubmissionError = "";

            DetailFrame = detailFrame;

            // We do not need to explictly load the initial data because if the ListView is visible
            // and the list is empty, it will automatically request more data.
            // ALSO: We make a new object because the events from the previous object can still be fired to change state
            // We could try to do some fancy event management to prevent that but I ain't got the time.
            _SubmissionList = new IncrementalLoadingList<SubmissionVM, IncrementalSubmissionList>(new IncrementalSubmissionList(App.VOAT_API, subverse));
        }

        ~MasterPageVM()
        {
            Debug.WriteLine("~MasterPageVM()");
        }

        #region Properties
        private string _Subverse;
        public string Subverse
        {
            get { return _Subverse; }
            set { SetProperty(ref _Subverse, value); }
        }

        private IncrementalLoadingList<SubmissionVM, IncrementalSubmissionList> _SubmissionList;
        public IncrementalLoadingList<SubmissionVM, IncrementalSubmissionList> SubmissionList
        {
            get { return _SubmissionList; }
        }

        private string _Sort;
        public string Sort { get { return _Sort; } set { SetProperty(ref _Sort, value); } }

        private bool _CanPost;
        public bool CanPost
        {
            get { return _CanPost; }
            set { SetProperty(ref _CanPost, value); }
        }

        private bool? _IsSubscribed;
        public bool? IsSubscribed { get { return _IsSubscribed; } set { SetProperty(ref _IsSubscribed, value); } }

        private bool _IsNewSubmissionPopupOpen;
        public bool IsNewSubmissionPopupOpen
        {
            get { return _IsNewSubmissionPopupOpen; }
            set { SetProperty(ref _IsNewSubmissionPopupOpen, value); }
        }

        private string _NewSubmissionTitle;
        public string NewSubmissionTitle
        {
            get { return _NewSubmissionTitle; }
            set { SetProperty(ref _NewSubmissionTitle, value); }
        }

        private string _NewSubmissionUrl;
        public string NewSubmissionUrl
        {
            get { return _NewSubmissionUrl; }
            set { SetProperty(ref _NewSubmissionUrl, value); }
        }

        private string _NewSubmissionContent;
        public string NewSubmissionContent
        {
            get { return _NewSubmissionContent; }
            set { SetProperty(ref _NewSubmissionContent, value); }
        }

        private string _NewSubmissionError;
        public string NewSubmissionError
        {
            get { return _NewSubmissionError; }
            set { SetProperty(ref _NewSubmissionError, value); }
        }

        private bool _PostingNewSubmission;
        public bool PostingNewSubmission
        {
            get { return _PostingNewSubmission; }
            set { SetProperty(ref _PostingNewSubmission, value); }
        }
        #endregion

        #region Private
        private bool validateTitle()
        {
            if (NewSubmissionTitle != null && NewSubmissionTitle.Length >= 5)
                return true;
            NewSubmissionError += "Submisstion title must be greater than 5 characters. ";
            return false;
        }

        private bool validateUrl()
        {
            Uri uri;
            bool success = Uri.TryCreate(NewSubmissionUrl, UriKind.Absolute, out uri);

            if (success) return true;

            NewSubmissionError += "Url must be a valid http(s) url. ";
            return false;
        }

        private bool validateContent()
        {
            if (NewSubmissionContent != null && NewSubmissionContent.Length != 0) return true;

            NewSubmissionError += "Due to a bug in Voat's API, content cannot be empty. Add a space perhaps? ";
            return false;
        }

        /// <summary>
        /// Helper method to post new submission and change current submission if post success
        /// </summary>
        /// <param name="submission"></param>
        private async void postSubmission(UserSubmission submission)
        {
            PostingNewSubmission = true;

            var r = await VOAT_API.PostSubmission(Subverse, submission);

            if (r.Success)
            {
                IsNewSubmissionPopupOpen = false;
                NewSubmissionTitle = "";
                NewSubmissionUrl = "";
                NewSubmissionContent = "";
                var submissionVM = SubmissionVM.FromApiSubmission(r.Data);
                SubmissionList.List.Insert(0, submissionVM);
                DetailFrame.Navigate(typeof(SubmissionCommentsPage), new SubmissionCommentsVM(submissionVM));
            }

            PostingNewSubmission = false;
        }
        #endregion

        #region Methods
        public void NewLink()
        {
            NewSubmissionError = "";
            if (validateTitle() && validateUrl())
            {
                var submission = new UserSubmission
                {
                    Title = NewSubmissionTitle,
                    Url = NewSubmissionUrl
                };
                postSubmission(submission);
            }
        }

        public void NewDiscussion()
        {
            NewSubmissionError = "";
            if (validateTitle() && validateContent())
            {
                var submission = new UserSubmission
                {
                    Title = NewSubmissionTitle,
                    Content = NewSubmissionContent
                };
                postSubmission(submission);
            }
        }

        public void Refresh()
        {
            VOAT_API.ResetSubmissionPage();
            SubmissionList.Dispose();
        }
        #endregion
    }
}
