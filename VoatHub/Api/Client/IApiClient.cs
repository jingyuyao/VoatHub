using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Data;

namespace VoatHub.Api
{
    /// <summary>
    /// Public facing interface for interacting with an API client.
    /// </summary>
    public interface IApiClient : IDisposable
    {
        /// <summary>
        /// Must be unique accross application.
        /// </summary>
        string ClientName { get; }

        Task<bool> Login(string username, string password);
        void Logout();
        bool LoggedIn { get; }

        Task<T> GetAsync<T>(Uri uri);
        Task<T> PostAsync<T>(Uri uri, IHttpContent content);
        Task<T> PutAsync<T>(Uri uri, IHttpContent content);
        Task<T> DeleteAsync<T>(Uri uri);
    }
}
