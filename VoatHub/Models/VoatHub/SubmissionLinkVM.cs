using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.VoatHub
{
    public class SubmissionLinkVM : SubmissionVM
    {
        public SubmissionLinkVM(SubmissionVM vm) : base(vm)
        {
            Uri = App.DEFAULT_URI;

            Uri temp;
            bool success = Uri.TryCreate(Submission.Url, UriKind.Absolute, out temp);
            if (success)
                Uri = temp;
        }

        private Uri _Uri;
        public Uri Uri
        {
            get { return _Uri; }
            set { SetProperty(ref _Uri, value); }
        }
    }
}
