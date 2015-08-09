using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Data;

namespace VoatHub.Api.Client
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
        /// <summary>
        /// Throws exception if not logged in.
        /// </summary>
        /// <exception cref="UnauthenticatedException"></exception>
        void EnsureLoggedIn();

        Task<T> GetAsync<T>(Uri uri);
        Task<T> PostAsync<T>(Uri uri, IHttpContent content);
        Task<T> PutAsync<T>(Uri uri, IHttpContent content);
        Task<T> DeleteAsync<T>(Uri uri);
    }
}
