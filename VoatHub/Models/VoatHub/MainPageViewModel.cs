using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using Windows.UI.Xaml.Controls;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// Contains the view model for the main page as well as methods to interact
    /// with the view model.
    /// </summary>
    public class MainPageViewModel : BindableBase
    {
        private string currentSubverse;
        private ApiSubmission currentPresentedSubmission;
        
        public string CurrentSubverse
        {
            get { return currentSubverse; }
            set { SetProperty(ref currentSubverse, value); }
        }

        public ApiSubmission CurrentPresentedSubmission
        {
            get { return currentPresentedSubmission; }
            set { SetProperty(ref currentPresentedSubmission, value); }
        }
    }
}
