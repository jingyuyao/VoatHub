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
        private MasterColumnViewModel masterColumn;
        private SubmissionViewModel currentSubmission;

        public MainPageViewModel()
        {
            masterColumn = new MasterColumnViewModel();
            // Fixes item source null binding errors.
            currentSubmission = new SubmissionViewModel();
        }

        public MasterColumnViewModel MasterColumn
        {
            get { return masterColumn; }
            set { SetProperty(ref masterColumn, value); }
        }
        
        public SubmissionViewModel CurrentSubmission
        {
            get { return currentSubmission; }
            set { SetProperty(ref currentSubmission, value); }
        }
    }
}
