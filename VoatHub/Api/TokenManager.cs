using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Security.Credentials;
using Windows.Web.Http;

using Newtonsoft.Json;

using VoatHub.Data;

namespace VoatHub.Api
{
    /// <summary>
    /// Provides async access to access token.
    /// </summary>
    public sealed class TokenManager
    {
        private IApiClient apiClient;
        private ApplicationDataContainer roamingSettings;
        private string accessTokenKey;
        private string accessTokenExpirationKey;
        /// <summary>
        /// Invariant: Must be null when not logged in.
        /// </summary>
        private string accessToken;
        /// <summary>
        /// Invariant: Must be default DateTime when not logged in.
        /// </summary>
        private DateTime accessTokenExpiration;

        /// <summary>
        /// A manager to manage refresh and retreival of tokens.
        /// <para>Actual logic to get token from server is provided by <see cref="IApiClient"/>.</para>
        /// </summary>
        /// <param name="apiClient">The client this manager is managing tokens for.</param>
        public TokenManager(IApiClient apiClient)
        {
            this.apiClient = apiClient;
            accessTokenKey = apiClient.ClientName + "AccessToken";
            accessTokenExpirationKey = apiClient.ClientName + "AccessTokenExpiration";

            roamingSettings = ApplicationData.Current.RoamingSettings;
            getDataFromSettings();
        }

        /// <summary>
        /// Get the accessToken of a client.
        /// <para>Refresh if logged in and token is expired.</para>
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns>The accessToken or null if there are none.</returns>
        public async Task<string> AccessToken()
        {
            if(apiClient.LoggedIn && (accessToken == null || Expired))
            {
                await updateTokenData();
            }

            return accessToken;
        }

        /// <summary>
        /// Return whether the token is expired or not.
        /// </summary>
        /// <returns></returns>
        public bool Expired
        {
            get
            {
                return accessTokenExpiration < DateTime.Now;
            }
        }

        /// <summary>
        /// Clears the token data from cache and settings.
        /// </summary>
        public void Clear()
        {
            accessToken = null;
            accessTokenExpiration = new DateTime();
            setDataToSettings();
        }

        /// <summary>
        /// Get data from roaming settings.
        /// </summary>
        /// <param name="clientName"></param>
        private void getDataFromSettings()
        {
            accessToken = (string)roamingSettings.Values[accessTokenKey];
            var binaryTime = roamingSettings.Values[accessTokenExpirationKey];

            accessTokenExpiration = binaryTime == null ? new DateTime() : DateTime.FromBinary((long)binaryTime);
        }

        /// <summary>
        /// Set current data to roaming settings.
        /// <para>accessTokenExpiration is stored as binary.</para>
        /// </summary>
        private void setDataToSettings()
        {
            roamingSettings.Values[accessTokenKey] = accessToken;
            roamingSettings.Values[accessTokenExpirationKey] = accessTokenExpiration.ToBinary();
        }

        /// <summary>
        /// Updates <code>clientData</code> by requsting info from the client's API.
        /// </summary>
        /// <param name="credential">Can be null.</param>
        private async Task updateTokenData()
        {
            var token = await apiClient.RetrieveToken();

            if (token != null)
            {
                accessToken = token.access_token;
                accessTokenExpiration = getExpiration(token.expires_in);
                setDataToSettings();
            }
        }

        /// <summary>
        /// Get a datetime with specific number of seconds in the future.
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private DateTime getExpiration(int seconds)
        {
            return DateTime.Now.Add(new TimeSpan(0, 0, seconds));
        }
    }
}
