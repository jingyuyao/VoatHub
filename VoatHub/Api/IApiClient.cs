using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Data;

namespace VoatHub.Api
{
    public interface IApiClient : IDisposable
    {
        /// <summary>
        /// Must be unique accross application.
        /// </summary>
        string ClientName { get; }
        /// <summary>
        /// Retrieve oauth token from API vendor.
        /// </summary>
        /// <returns></returns>
        Task<ApiToken> RetrieveToken();

        void Login(string username, string password);
        void Logout();
        bool LoggedIn { get; }

        Task<T> GetAsync<T>(Uri uri);
        Task<T> PostAsync<T>(Uri uri, IHttpContent content);
    }
}
