using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;

namespace VoatHub.Api
{
    /// <summary>
    /// Manages credentials to login to various clients.
    /// </summary>
    /// <remarks>Currently only supports one user per client.</remarks>
    class CredentialManager
    {
        private PasswordVault vault;

        public CredentialManager()
        {
            vault = new PasswordVault();
        }

        /// <summary>
        /// Login with the given credentials and store the credentials in credential locker.
        /// <para>Also obtains the token data from either roaming settings or as a blocking operation
        /// from the API.</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Login(string client, string username, string password)
        {
            Logout(client);
            vault.Add(new PasswordCredential(client, username, password));
        }

        /// <summary>
        /// Removes Voat username from local data, password from credential locker
        /// and token data.
        /// </summary>
        /// <param name="client"></param>
        public void Logout(string client)
        {
            var credentials = vault.FindAllByResource(client);

            foreach (var credential in credentials)
            {
                vault.Remove(credential);
            }
        }

        /// <summary>
        /// Get the first credentials in the vault or return null.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public PasswordCredential GetCredential(string client)
        {
            var credentials = vault.FindAllByResource(client);

            if (credentials.Count > 0)
            {
                return credentials[0];
            }

            return null;
        }
    }
}
