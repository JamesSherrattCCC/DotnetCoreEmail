using EmailReader;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmailDaemon.Auth
{
    /// <summary>
    /// Retrieves the authorisation tokens required to use microsoft graph.
    /// </summary>
    public class AuthApp : IAuthenticationProvider
    {
        protected static IPublicClientApplication _msalClient;
        protected IAccount _userAccount;
        protected static AuthConfig _config = AuthConfig.Config;

        public static AuthConfig Config { get => _config; }
        public static IPublicClientApplication App { get; } = GetApp();
        public IAccount UserAccount { get { 
                if (_userAccount == null)
                    _ = GetAccessToken().Result;
                return _userAccount;
            } }

        /// <summary>
        /// Get the application which will be used to connect to Azure + MS graph.
        /// </summary>
        /// <returns>Client application app, used for getting an authorisation token.</returns>
        protected static IPublicClientApplication GetApp()
        {
            // Public client application, for getting a (multitenant) token for any microsoft account.
            var app =  PublicClientApplicationBuilder.Create(Config.ClientId)
                .WithRedirectUri("http://localhost")
                .WithAuthority(AzureCloudInstance.AzurePublic, Config.Tenant)
                .Build();
            // Cache tokens for re-use. These are stored encrypted so only the user can access them.
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
            return app;
        }

        /// <summary>
        /// Get the authorisation token for MS graph.
        /// </summary>
        /// <returns>the authorisation token as a string.</returns>
        public async Task<string> GetAccessToken()
        {
            AuthenticationResult result = null;
            try
            {
                _userAccount = App.GetAccountsAsync().Result.FirstOrDefault();
                if (_userAccount != null)
                {
                    try
                    {
                        result = await App.AcquireTokenSilent(AuthConfig.Config.Scopes, _userAccount).ExecuteAsync();
                    }
                    
                    catch (MsalUiRequiredException ex)
                    {
                        // Sign in using the ui required.
                        result = await App.AcquireTokenInteractive(AuthConfig.Config.Scopes).ExecuteAsync();
                        _userAccount = result.Account;
                    }
                }
                else
                {
                    // Sign in using the UI required (repeated code because the first one happens due to an
                    // error and the second time due to no account authorisation tokens having been cached).
                    // TODO: make the code more DRY-Compiant.
                    result = await App.AcquireTokenInteractive(AuthConfig.Config.Scopes).ExecuteAsync();
                    _userAccount = result.Account;
                }
                return result.AccessToken;

            }
            catch (MsalUiRequiredException ex)
            {
                // The application does not have sufficient permissions
                // - did you declare enough app permissions in during the app creation?
                // - did the tenant admin needs to grant permissions to the application.
                Console.WriteLine("Insufficient Permissions.");
            }
            catch (MsalServiceException ex) when (ex.Message.Contains("AADSTS70011"))
            {
                // Invalid scope. The scope has to be of the form "https://resourceurl/.default"
                // Mitigation: change the scope to be as expected !
                Console.WriteLine("Invalid scope");
            }
            return null;
        }

        /// <summary>
        /// Helper function to add an authorisation token to the header of a http message.
        /// </summary>
        /// <param name="request">The request to authorise</param>
        /// <returns>Task. (The task adds a header to the request passed to the task.)</returns>
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}
