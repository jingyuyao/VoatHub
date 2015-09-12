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
        private bool loadInitiated = false;

        public MainPageVM()
        {
            // From left to right side of the page

            Navlist = new List<NavMenuItem>(new[]
            {
                new NavMenuItem
                {
                    Symbol = Symbol.Home,
                    Label = "Front Page"
                },
                new NavMenuItem
                {
                    Symbol = Symbol.AllApps,
                    Label = "All"
                }
            });

            OptionList = new List<NavMenuItem>(new[]
            {
                new NavMenuItem
                {
                    Symbol = Symbol.Contact,
                    Label = "Account"
                },
                new NavMenuItem
                {
                    Symbol = Symbol.Setting,
                    Label = "Settings"
                }
            });

            Subscriptions = new LoadingList<ApiSubscription>();
        }

        #region Properties
        private List<NavMenuItem> _Navlist;
        public List<NavMenuItem> Navlist
        {
            get { return _Navlist; }
            set { SetProperty(ref _Navlist, value); }
        }

        private List<NavMenuItem> _OptionList;
        public List<NavMenuItem> OptionList
        {
            get { return _OptionList; }
            set { SetProperty(ref _OptionList, value); }
        }

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
        
        public async void LoadSubscriptions()
        {
            if (!loadInitiated)
            {
                loadInitiated = true;

                var subscriptions = await VOAT_API.UserSubscriptions(VOAT_API.UserName);
                
                if (subscriptions.Success && subscriptions.Data != null)
                {
                    foreach (var item in subscriptions.Data)
                    {
                        Subscriptions.List.Add(item);
                    }
                }
                Subscriptions.Loading = false;
            }
        }

        public bool IsSubscribed(string subverse)
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

        public bool CanSubmit(string subverse)
        {
            if (subverse == "_front" || subverse == "_all")
                return false;
            else if (Subscriptions != null)
            {
                foreach (var sub in Subscriptions.List)
                {
                    if (string.Equals(subverse, sub.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return sub.Type == SubscriptionType.Subverse;
                    }
                }
            }

            // current subverse is not one of the special ones and its not in the subscription list that contains all the sets.
            return true;
        }
        #endregion
    }
}
