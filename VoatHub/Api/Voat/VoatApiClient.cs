using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Web.Http;

using VoatHub.Api.Client;
using VoatHub.Models.Voat;

namespace VoatHub.Api.Voat
{
    /// <summary>An ApiClient tailored for Voat's API.
    /// <para>
    /// Adds Voat-ApiKey on every request and manages token, crendential and throttle.
    /// The user of this client is responsible to handle errors returned by this class
    /// as well as checking login status before making calls.
    /// </para>
    /// </summary>
    public sealed class VoatApiClient : JsonApiClient
    {
        private readonly string _clientName = "voatClient";
        private Uri tokenUri;

        public VoatApiClient(string apiKey, string tokenUri)
        {
            addHeader("Voat-ApiKey", apiKey);
            this.tokenUri = new Uri(tokenUri);
        }

        public override string ClientName
        {
            get
            {
                return _clientName;
            }
        }

        protected override async Task<ApiToken> retrieveToken(string username, string password)
        {
            Debug.WriteLine("Retrieving token...", "VoatApiClient");
            
            // Does not need authorization header.
            // Calling setAuthorizationHeader can also cause circular reference

            var properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("grant_type", "password"));
            properties.Add(new KeyValuePair<string, string>("username", username));
            properties.Add(new KeyValuePair<string, string>("password", password));

            var content = new HttpFormUrlEncodedContent(properties);
            var token = await PostAsync<ApiToken>(tokenUri, content);

            Debug.WriteLine(token, "VoatApiClient");

            return token;
        }

        protected override async Task<ApiToken> refreshToken()
        {
            var credential = credentialManager.Credential;
            if (credential != null)
            {
                return await retrieveToken(credential.UserName, credential.Password);
            }

            return null;
        }
    }
}
