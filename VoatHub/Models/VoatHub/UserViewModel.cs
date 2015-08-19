using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    public class UserViewModel : BindableBase
    {
        private ApiUserInfo userInfo;
        private LoadingList<ApiSubscription> subscriptions;

        public UserViewModel()
        {
            subscriptions = new LoadingList<ApiSubscription>();
        }

        public ApiUserInfo UserInfo
        {
            get { return userInfo; }
            set { SetProperty(ref userInfo, value); }
        }

        public LoadingList<ApiSubscription> Subscriptions
        {
            get { return subscriptions; }
            set { SetProperty(ref subscriptions, value); }
        }
    }
}
