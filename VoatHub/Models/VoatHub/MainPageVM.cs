using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using Windows.UI.Xaml.Controls;

using VoatHub.Models.Voat.v1;
using System.Collections.ObjectModel;

using VoatHub.Models.VoatHub.LoadingList;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// Contains the view model for the main page as well as methods to interact
    /// with the view model.
    /// </summary>
    public class MainPageVM : BindableBase
    {
        private VoatApi VOAT_API = App.VOAT_API;

        public MainPageVM()
        {
            // From left to right side of the page

            Navlist = new List<NavMenuItem>(new[]
            {
                new NavMenuItem()
                {
                    Symbol = Symbol.Home,
                    Label = "Front Page"
                },
                new NavMenuItem()
                {
                    Symbol = Symbol.AllApps,
                    Label = "All"
                }
            });
            Subscriptions = new LoadingList<ApiSubscription>();

            // SubmissionList is left out because it is set by ChangeSubverse()
            SubmissionSort = "Hot";

            // TODO: loading from settings
            ChangeSubverse("_front");
        }

        #region Properties
        private List<NavMenuItem> _Navlist;
        public List<NavMenuItem> Navlist
        {
            get { return _Navlist; }
            set { SetProperty(ref _Navlist, value); }
        }

        private string _CurrentSubverse;
        public string CurrentSubverse
        {
            get { return _CurrentSubverse; }
            set { SetProperty(ref _CurrentSubverse, value); }
        }

        private IncrementalLoadingList<ApiSubmission, IncrementalSubmissionList> _SubmissionList;
        public IncrementalLoadingList<ApiSubmission, IncrementalSubmissionList> SubmissionList
        {
            get { return _SubmissionList; }
            set { Contract.Requires(value != null); SetProperty(ref _SubmissionList, value); }
        }

        private bool? _CurrentlySubscribed;
        public bool? CurrentlySubscribed { get { return _CurrentlySubscribed; } set { SetProperty(ref _CurrentlySubscribed, value); } }

        private string _SubmissionSort;
        public string SubmissionSort { get { return _SubmissionSort; } set { SetProperty(ref _SubmissionSort, value); } }

        private ApiUserInfo _UserInfo;
        public ApiUserInfo UserInfo
        {
            get { return _UserInfo; }
            set { SetProperty(ref _UserInfo, value); }
        }

        private LoadingList<ApiSubscription> _Subscriptions;
        public LoadingList<ApiSubscription> Subscriptions
        {
            get { return _Subscriptions; }
            set { SetProperty(ref _Subscriptions, value); }
        }
        #endregion

        #region Methods
        public void ChangeSubverse(string subverse)
        {
            // We do not need to explictly load the initial data because if the ListView is visible
            // and the list is empty, it will automatically request more data.
            // ALSO: We make a new object because the events from the previous object can still be fired to change state
            // We could try to do some fancy event management to prevent that but I ain't got the time.
            SubmissionList = new IncrementalLoadingList<ApiSubmission, IncrementalSubmissionList>(new IncrementalSubmissionList(VOAT_API, subverse));
            CurrentlySubscribed = isSubscribed(subverse);
            CurrentSubverse = subverse;
        }

        public void RefreshCurrentSubverse()
        {
            ChangeSubverse(CurrentSubverse);
        }

        public async void LoadSubscriptions()
        {
            if (Subscriptions.Loading)
            {
                var subscriptions = await VOAT_API.UserSubscriptions(VOAT_API.UserName);
                
                if (subscriptions.Success && subscriptions.Data != null)
                    Subscriptions.List = new ObservableCollection<ApiSubscription>(subscriptions.Data);

                Subscriptions.Loading = false;
            }
        }

        private bool isSubscribed(string subverse)
        {
            if (subverse == "_front" || subverse == "_all")
            {
                return true;
            }
            else if (Subscriptions != null)
            {
                foreach (var sub in Subscriptions.List)
                {
                    if (string.Equals(subverse, sub.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}
