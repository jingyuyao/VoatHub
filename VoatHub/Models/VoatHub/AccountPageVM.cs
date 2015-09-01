using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoatHub.Models.Voat.v1;

namespace VoatHub.Models.VoatHub
{
    public class AccountPageVM : BindableBase
    {
        public AccountPageVM()
        {
            Loading = true;
            load();
        }

        private async void load()
        {
            var r = await App.VOAT_API.UserInfo(App.VOAT_API.UserName);
            if (r != null && r.Success)
                UserInfo = r.Data;
            Loading = false;
        }

        private ApiUserInfo _UserInfo;
        public ApiUserInfo UserInfo
        {
            get { return _UserInfo; }
            set { SetProperty(ref _UserInfo, value); }
        }

        private bool _Loading;
        public bool Loading
        {
            get { return _Loading; }
            set { SetProperty(ref _Loading, value); }
        }
    }
}
