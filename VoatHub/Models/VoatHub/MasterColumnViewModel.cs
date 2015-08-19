using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VoatHub.Models.Voat.v1;
using Windows.UI.Xaml.Controls;

namespace VoatHub.Models.VoatHub
{
    public class MasterColumnViewModel : BindableBase
    {
        private string currentSubverse;
        private bool? currentlySubscribed;
        private LoadingList<ApiSubmission> currentSubmissions;

        public MasterColumnViewModel()
        {
            currentSubmissions = new LoadingList<ApiSubmission>();
            currentSubverse = "Loading...";
            currentlySubscribed = false;
        }

        public string CurrentSubverse
        {
            get { return currentSubverse; }
            set { SetProperty(ref currentSubverse, value); }
        }

        public LoadingList<ApiSubmission> CurrentSubmissions
        {
            get { return currentSubmissions; }
            set { SetProperty(ref currentSubmissions, value); }
        }

        public bool? CurrentlySubscribed { get { return currentlySubscribed; } set { SetProperty(ref currentlySubscribed, value); } }
    }
}
