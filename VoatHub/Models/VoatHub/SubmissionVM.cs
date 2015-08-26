using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Models.Voat.v1;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace VoatHub.Models.VoatHub
{
    public class SubmissionVM : BindableBase
    {
        private static readonly Uri DEFAULT_LINK_IMAGE_URI = new Uri("ms-appx:Assets/LinkThumbnailPlaceholder.png", UriKind.Absolute);
        private static readonly Uri DEFAULT_SELF_IMAGE_URI = new Uri("ms-appx:Assets/SelfThumbnailPlaceholder.png", UriKind.Absolute);

        private ApiSubmission _Submission;
        public ApiSubmission Submission
        {
            get { return _Submission; }
            set { SetProperty(ref _Submission, value); }
        }

        private ImageSource _ImageSource;
        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set { SetProperty(ref _ImageSource, value); }
        }

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
