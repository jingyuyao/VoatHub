using System;
using System.Diagnostics;
using Windows.Security.Credentials;

namespace VoatHub.Api.Client
{
    /// <summary>
    /// Manages credentials to login to various clients.
    /// </summary>
    /// <remarks>Currently only supports one user per client.</remarks>
    public sealed class CredentialManager
    {
        private PasswordVault vault;
        private string clientName;
        private PasswordCredential credential;  //Cache to avoid look up

        public CredentialManager(string clientName)
        {
            vault = new PasswordVault();
            this.clientName = clientName;
            credential = null;
        }

        /// <summary>
        /// Login with the given credentials and store the credentials in credential locker.
        /// <para>Also obtains the token data from either roaming settings or as a blocking operation
        /// from the API.</para>
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void Login(string username, string password)
        {
            Logout();
            Debug.WriteLine("Logged in " + username, "CredentialManager");
            credential = new PasswordCredential(clientName, username, password);
            vault.Add(credential);
        }

        /// <summary>
        /// Removes Voat username from local data, password from credential locker
        /// and token data.
        /// </summary>
        public void Logout()
        {
            var allCredentials = vault.RetrieveAll();

            foreach (var credential in allCredentials)
            {
                if (credential.Resource == clientName)
                {
                    vault.Remove(credential);
                    Debug.WriteLine("Removed " + credential.UserName, this.GetType().Name);
                }
            }
            credential = null;
        }

        /// <summary>
        /// Return whether or now the user is logged in by checking existence of credential.
        /// </summary>
        public bool LoggedIn
        {
            get
            {
                return credential != null;
            }
        }

        /// <summary>
        /// Get the first credentials in the vault or return null.
        /// <para>Note: <see cref="PasswordCredential.RetrievePassword"/> has already been called.</para>
        /// </summary>
        /// <returns></returns>
        public PasswordCredential Credential
        {
            get
            {
                if (credential == null)
                {
                    var allCredentials = vault.RetrieveAll();

                    foreach (var credential in allCredentials)
                    {
                        if(credential.Resource == clientName)
                        {
                            credential.RetrievePassword();
                            return credential;
                        }
                    }

                    return null;
                }

                return credential;
            }
        }
    }
}
