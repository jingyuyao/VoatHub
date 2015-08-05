using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Web.Http;

namespace VoatHub.Api
{
    interface OauthClient
    {
        T GetOauthToken<T>();
    }
}
