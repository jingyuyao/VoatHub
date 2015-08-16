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
        private List<ApiSubmission> currentSubmissionsList;
        private bool loadingSubmissions;

        public MasterColumnViewModel()
        {
            currentSubmissionsList = new List<ApiSubmission>();
            currentSubverse = "Loading...";
        }

        public string CurrentSubverse
        {
            get { return currentSubverse; }
            set { SetProperty(ref currentSubverse, value); }
        }

        public bool LoadingSubmissions
        {
            get { return loadingSubmissions; }
            set { SetProperty(ref loadingSubmissions, value); }
        }

        public List<ApiSubmission> CurrentSubmissionsList
        {
            get { return currentSubmissionsList; }
            set { SetProperty(ref currentSubmissionsList, value); }
        }
    }
}
