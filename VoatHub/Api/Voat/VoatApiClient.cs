using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Windows.Storage;
using Windows.Security.Credentials;

using Newtonsoft.Json;

using VoatHub.Data;

namespace VoatHub.Api
{
    /// <summary>An HttpClient tailored for Voat's API.
    /// <para>
    /// Adds Voat-ApiKey on every request and deserializes Json response into
    /// the approperiate class.
    /// </para>
    /// </summary>
    public class VoatApiClient : JsonApiClient
    {
        private Uri tokenUri;

        public VoatApiClient(string apiKey, string tokenUri)
        {
            httpClient.DefaultRequestHeaders.Add("Voat-ApiKey", apiKey);
            this.tokenUri = new Uri(tokenUri);
        }

        public override string ClientName
        {
            get
            {
                return "voatClient";
            }
        }

        public override async Task<ApiToken> RetrieveToken()
        {
            Debug.WriteLine("Retrieving token...", "ApiClient");
            await Task.Delay(throttleManager.WaitTime);
            // Does not need authorization header.
            // Calling setAuthorizationHeader can also cause circular reference

            var credential = credentialManager.Credential;

            var properties = new List<KeyValuePair<string, string>>();
            properties.Add(new KeyValuePair<string, string>("grant_type", "password"));
            properties.Add(new KeyValuePair<string, string>("username", credential.UserName));
            properties.Add(new KeyValuePair<string, string>("password", credential.Password));

            var content = new HttpFormUrlEncodedContent(properties);
            var response = await httpClient.PostAsync(tokenUri, content);
            var responseString = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ApiToken>(responseString);
            Debug.WriteLine(token, "ApiClient");
            return token;
        }
    }
}
