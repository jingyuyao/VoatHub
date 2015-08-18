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
        private List<ApiSubscription> subscriptions;

        public ApiUserInfo UserInfo
        {
            get { return userInfo; }
            set { SetProperty(ref userInfo, value); }
        }

        public List<ApiSubscription> Subscriptions
        {
            get { return subscriptions; }
            set { SetProperty(ref subscriptions, value); }
        }
    }
}
