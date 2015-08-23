using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using VoatHub.Api.Voat;
using VoatHub.Models.Voat;
using Windows.UI.Xaml.Controls;

using VoatHub.Models.Voat.v1;
using System.Collections.ObjectModel;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// Contains the view model for the main page as well as methods to interact
    /// with the view model.
    /// </summary>
    public class MainPageVM : BindableBase
    {
        private VoatApi api;

        public MainPageVM(VoatApi api)
        {
            this.api = api;

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

            SubmissionList = new LoadingList<ApiSubmission>();
            CurrentSubverse = "Loading...";
            CurrentlySubscribed = false;
            SubmissionSort = CommentSort = "Hot";

            // Fixes item source null binding errors.
            CurrentSubmission = new SubmissionVM(api);

            initialSetup();
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

        private LoadingList<ApiSubmission> _SubmissionList;
        public LoadingList<ApiSubmission> SubmissionList
        {
            get { return _SubmissionList; }
            set { Contract.Requires(value != null); SetProperty(ref _SubmissionList, value); }
        }

        private bool? _CurrentlySubscribed;
        public bool? CurrentlySubscribed { get { return _CurrentlySubscribed; } set { SetProperty(ref _CurrentlySubscribed, value); } }

        private SubmissionVM _CurrentSubmission;
        public SubmissionVM CurrentSubmission
        {
            get { return _CurrentSubmission; }
            set { Contract.Requires(value != null); SetProperty(ref _CurrentSubmission, value); }
        }

        private string _SubmissionSort;
        public string SubmissionSort { get { return _SubmissionSort; } set { SetProperty(ref _SubmissionSort, value); } }

        private string _CommentSort;
        public string CommentSort { get { return _CommentSort; } set { SetProperty(ref _CommentSort, value); } }

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
        private async void initialSetup()
        {
            // TODO: Load from saved settings
            ChangeSubverse("_front");

            var subscriptions = await api.UserSubscriptions(api.UserName);

            List<ApiSubscription> list = null;
            if (subscriptions.Success)
                list = subscriptions.Data;

            Subscriptions.List = new ObservableCollection<ApiSubscription>(list);

            //var userInfo = await voatApi.UserInfo(voatApi.UserName);
            //if (userInfo.Success)
            //    ViewModel.User.UserInfo = userInfo.Data;
        }

        public void ChangeSubverse(string subverse)
        {
            CurrentlySubscribed = isSubscribed(subverse);
            CurrentSubverse = subverse;
            // We do not need to explictly load the initial data because if the ListView is visible
            // and the list is empty, it will automatically request more data.
            SubmissionList.List = new IncrementalSubmissionList(api, subverse);
        }

        public void RefreshCurrentSubverse()
        {
            ChangeSubverse(CurrentSubverse);
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
