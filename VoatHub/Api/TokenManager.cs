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
        private const string VOAT_ACCESS_TOKEN = "voatAccessToken";
        private const string VOAT_ACCESS_TOKEN_EXPIRATION = "voatAccessTokenExpiration";

        private Uri tokenUri;
        private HttpClient httpClient;
        private CredentialManager credentialManager;
        private ApplicationDataContainer roamingSettings;
        private string voatAccessToken;  // Invariant: Must be null when not logged in.
        private DateTime voatAccessTokenExpiration;  // Invariant: Must be default DateTime when not logged in.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenUri">Absolute path to the token endpoint.</param>
        /// <param name="httpClient"></param>
        /// <param name="credentialManager"></param>
        public TokenManager(string tokenUri, HttpClient httpClient, CredentialManager credentialManager)
        {
            this.tokenUri = new Uri(tokenUri);
            this.httpClient = httpClient;
            this.credentialManager = credentialManager;
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
            if(credentialManager.LoggedIn && (voatAccessToken == null || Expired))
            {
                await updateTokenData();
            }

            return voatAccessToken;
        }

        /// <summary>
        /// Return whether the token is expired or not.
        /// </summary>
        /// <returns></returns>
        public bool Expired
        {
            get
            {
                return voatAccessTokenExpiration < DateTime.Now;
            }
        }

        /// <summary>
        /// Clears the token data from cache and settings.
        /// </summary>
        public void Clear()
        {
            voatAccessToken = null;
            voatAccessTokenExpiration = new DateTime();
            setDataToSettings();
        }

        /// <summary>
        /// Get data from roaming settings.
        /// </summary>
        /// <param name="clientName"></param>
        private void getDataFromSettings()
        {
            voatAccessToken = (string)roamingSettings.Values[VOAT_ACCESS_TOKEN];
            var binaryTime = roamingSettings.Values[VOAT_ACCESS_TOKEN_EXPIRATION];

            voatAccessTokenExpiration = binaryTime == null ? new DateTime() : DateTime.FromBinary((long)binaryTime);
        }

        /// <summary>
        /// Set current data to roaming settings.
        /// <para>accessTokenExpiration is stored as binary.</para>
        /// </summary>
        private void setDataToSettings()
        {
            roamingSettings.Values[VOAT_ACCESS_TOKEN] = voatAccessToken;
            roamingSettings.Values[VOAT_ACCESS_TOKEN_EXPIRATION] = voatAccessTokenExpiration.ToBinary();
        }

        /// <summary>
        /// Updates <code>clientData</code> by requsting info from the client's API.
        /// </summary>
        /// <param name="credential">Can be null.</param>
        private async Task updateTokenData()
        {
            var credential = credentialManager.Credential;

            var properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("grant_type", "password"));
            properties.Add(new KeyValuePair<string, string>("username", credential.UserName));
            properties.Add(new KeyValuePair<string, string>("password", credential.Password));

            var content = new HttpFormUrlEncodedContent(properties);

            // Blocking operations
            var response = await httpClient.PostAsync(tokenUri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ApiToken>(responseString);

            voatAccessToken = token.access_token;
            voatAccessTokenExpiration = getExpiration(token.expires_in);
            setDataToSettings();
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
