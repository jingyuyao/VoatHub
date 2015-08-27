using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.VoatHub
{
    public class WebVM : BindableBase
    {
        public static readonly Uri DEFAULT_URI = new Uri("about:blank", UriKind.Absolute);

        public WebVM()
        {
            Uri = DEFAULT_URI;
        }

        public WebVM(string url) : this()
        {
            Uri temp;
            bool success = Uri.TryCreate(url, UriKind.Absolute, out temp);
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
