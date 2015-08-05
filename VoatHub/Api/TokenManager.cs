using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Security.Credentials;

namespace VoatHub.Api
{
    class TokenManager
    {
        private CredentialManager credentialManager;
        private ApplicationDataContainer roamingSettings;
        private string clientName;
        private ApplicationDataCompositeValue clientData;  // Invariant non-null

        public TokenManager(string clientName)
        {
            credentialManager = new CredentialManager();
            roamingSettings = ApplicationData.Current.RoamingSettings;
            this.clientName = clientName;
            clientData = getOrCreateClient(clientName);
        }

        /// <summary>
        /// Try to get client data from settings. If there is none, try to get data from
       ///  api if user is logged in. Else return empty.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns></returns>
        private ApplicationDataCompositeValue getOrCreateClient(string clientName)
        {
            clientData = (ApplicationDataCompositeValue) roamingSettings.Values[clientName];
            if (clientData == null)
            {
                clientData = new ApplicationDataCompositeValue();

                updateClientData(credentialManager.GetCredential(clientName));
                syncClientData();

                return clientData;
            }
            return clientData;
        }

        private void syncClientData()
        {
            roamingSettings.Values[clientName] = clientData;
        }

        /// <summary>
        /// Get the accessToken of a client.
        /// </summary>
        /// <param name="clientName"></param>
        /// <returns>The accessToken or null if there are none.</returns>
        public string GetAccessToken()
        {
            DateTime? accessTokenExpiration = (DateTime)clientData["accessTokenExpiration"];

            if (accessTokenExpiration != null && accessTokenExpiration > DateTime.Now)
            {
                updateClientData(credentialManager.GetCredential(clientName));
            }

            return (string)clientData["accessToken"];
        }

        /// <summary>
        /// Updates <code>clientData</code> by requsting info from the client's API.
        /// </summary>
        /// <param name="credential">Can be null.</param>
        private void updateClientData(PasswordCredential credential)
        {
            if (credential != null)
            {
                // TODO Use the token api to update clientData

                syncClientData();
            }
        }
    }
}
