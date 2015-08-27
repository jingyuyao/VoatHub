using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat.v1;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VoatHub.Models.VoatHub
{
    public class SubmissionVM : BindableBase
    {
        private VoatApi VOAT_API = App.VOAT_API;
        private static readonly Uri DEFAULT_LINK_IMAGE_URI = new Uri("ms-appx:Assets/LinkThumbnailPlaceholder.png", UriKind.Absolute);
        private static readonly Uri DEFAULT_SELF_IMAGE_URI = new Uri("ms-appx:Assets/SelfThumbnailPlaceholder.png", UriKind.Absolute);

        public SubmissionVM()
        {

        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="vm"></param>
        public SubmissionVM(SubmissionVM vm)
        {
            Submission = vm.Submission;
            ImageSource = vm.ImageSource;
        }

        #region Properties
        private ApiSubmission _Submission;
        public ApiSubmission Submission
        {
            get { return _Submission; }
            set { SetProperty(ref _Submission, value); }
        }

        private bool? _VoteStatus;
        public bool? VoteStatus
        {
            get { return _VoteStatus; }
            set { SetProperty(ref _VoteStatus, value); }
        }

        private ImageSource _ImageSource;
        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set { SetProperty(ref _ImageSource, value); }
        }
        #endregion

        #region
        public async void UpVote()
        {
            if (VoteStatus == true) VoteStatus = null;
            else VoteStatus = true;

            var result = await VOAT_API.PostVoteRevokeOnRevote("submission", Submission.ID, 1, true);
            if (result == null || !result.Success) VoteStatus = null;

            // TODO: display some sort of error message when voting failed.
        }

        public async void DownVote()
        {
            if (VoteStatus == false) VoteStatus = null;
            else VoteStatus = false;

            var result = await VOAT_API.PostVoteRevokeOnRevote("submission", Submission.ID, -1, true);
            if (result == null || !result.Success) VoteStatus = null;
        }

        #endregion

        /// <summary>
        /// Creates a <see cref="SubmissionVM"/> from <see cref="ApiSubmission"/>.
        /// </summary>
        /// <param name="submission"></param>
        /// <returns></returns>
        public static SubmissionVM FromApiSubmission(ApiSubmission submission)
        {
            var vm = new SubmissionVM();
            vm.Submission = submission;
            
            Uri imageUri = null;

            if (submission.Type == ApiSubmissionType.Self)
                imageUri = DEFAULT_SELF_IMAGE_URI;
            else
            {
                Uri temp;
                bool success = Uri.TryCreate(submission.Thumbnail, UriKind.Absolute, out temp);
                imageUri = success ? temp : DEFAULT_LINK_IMAGE_URI;
            }

            vm.ImageSource = new BitmapImage(imageUri);

            return vm;
        }
    }
}
