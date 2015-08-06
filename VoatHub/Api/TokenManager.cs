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
    public class TokenManager
    {
        private const string VOAT_ACCESS_TOKEN = "voatAccessToken";
        private const string VOAT_ACCESS_TOKEN_EXPIRATION = "voatAccessTokenExpiration";

        private Uri tokenUri;
        private HttpClient httpClient;
        private CredentialManager credentialManager;
        private ApplicationDataContainer roamingSettings;
        private string voatAccessToken;
        private DateTime voatAccessTokenExpiration;

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
        /// Get the accessToken of a client. S
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns>The accessToken or null if there are none.</returns>
        public string AccessToken
        {
            get
            {
                if (voatAccessToken == null || Expired)
                {
                    updateTokenData();
                }
                return voatAccessToken;
            }
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
        /// Try to get client data from settings. If there is none, try to get data from
        ///  api if user is logged in. Else return empty.
        /// </summary>
        /// <param name="clientName"></param>
        private void getDataFromSettings()
        {
            voatAccessToken = (string)roamingSettings.Values[VOAT_ACCESS_TOKEN];
            var binaryTime = roamingSettings.Values[VOAT_ACCESS_TOKEN_EXPIRATION];

            voatAccessTokenExpiration = binaryTime == null ? new DateTime() : DateTime.FromBinary((long)binaryTime);

            if (voatAccessToken == null || voatAccessTokenExpiration == null)
            {
                updateTokenData();
            }
        }

        /// <summary>
        /// Updates <code>clientData</code> by requsting info from the client's API.
        /// </summary>
        /// <param name="credential">Can be null.</param>
        private void updateTokenData()
        {
            if (credentialManager.LoggedIn)
            {
                var credential = credentialManager.Credential;

                var properties = new List<KeyValuePair<string, string>>();
                properties.Add(new KeyValuePair<string, string>("grant_type", "password"));
                properties.Add(new KeyValuePair<string, string>("username", credential.UserName));
                properties.Add(new KeyValuePair<string, string>("password", credential.Password));

                var content = new HttpFormUrlEncodedContent(properties);

                // Blocking operations
                var response = httpClient.PostAsync(tokenUri, content).AsTask().Result;
                var responseString = response.Content.ReadAsStringAsync().AsTask().Result;
                var token = JsonConvert.DeserializeObject<ApiToken>(responseString);

                voatAccessToken = token.access_token;
                voatAccessTokenExpiration = getExpiration(token.expires_in);
                syncWithSettings();
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

        private void syncWithSettings()
        {
            roamingSettings.Values[VOAT_ACCESS_TOKEN] = voatAccessToken;
            roamingSettings.Values[VOAT_ACCESS_TOKEN_EXPIRATION] = voatAccessTokenExpiration.ToBinary();
        }
    }
}
