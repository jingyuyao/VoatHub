using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Models.Voat.v1;
using VoatHub.Models.VoatHub.LoadingList;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace VoatHub.Models.VoatHub
{
    public class MasterPageVM : BindableBase
    {
        /// <summary>
        /// Allows MasterPage to control DetailFrame
        /// </summary>
        public Frame DetailFrame { get; }

        public MasterPageVM(string subverse, bool isSubscribed, bool canPost, Frame detailFrame)
        {
            Subverse = subverse;
            IsSubscribed = isSubscribed;
            CanPost = canPost;
            DetailFrame = detailFrame;

            // We do not need to explictly load the initial data because if the ListView is visible
            // and the list is empty, it will automatically request more data.
            // ALSO: We make a new object because the events from the previous object can still be fired to change state
            // We could try to do some fancy event management to prevent that but I ain't got the time.
            SubmissionList = new IncrementalLoadingList<SubmissionVM, IncrementalSubmissionList>(new IncrementalSubmissionList(App.VOAT_API, subverse));
            Sort = "Hot";
        }

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
            set { Contract.Requires(value != null); SetProperty(ref _SubmissionList, value); }
        }

        private bool? _IsSubscribed;
        public bool? IsSubscribed { get { return _IsSubscribed; } set { SetProperty(ref _IsSubscribed, value); } }

        private string _Sort;
        public string Sort { get { return _Sort; } set { SetProperty(ref _Sort, value); } }

        private bool _CanPost;
        public bool CanPost
        {
            get { return _CanPost; }
            set { SetProperty(ref _CanPost, value); }
        }

        public void Refresh()
        {
            SubmissionList = new IncrementalLoadingList<SubmissionVM, IncrementalSubmissionList>(new IncrementalSubmissionList(App.VOAT_API, Subverse));
        }
    }
}
