using EmailReader;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmailDaemon.Auth
{
    class AuthApp : IAuthenticationProvider
    {
        private static IPublicClientApplication _msalClient;
        private IAccount _userAccount = null;

        public static AuthConfig Config { get; } = AuthConfig.Config;
        public static IPublicClientApplication App { get; } = GetApp();
        public IAccount UserAccount { get { return _userAccount; } }

        public AuthApp()
        {
            _ = GetAccessToken();
        }

        private static IPublicClientApplication GetApp()
        {
            var app =  PublicClientApplicationBuilder.Create(Config.ClientId)
                .WithRedirectUri("http://localhost")
                .WithAuthority(AzureCloudInstance.AzurePublic, Config.Tenant)
                .Build();
            TokenCacheHelper.EnableSerialization(app.UserTokenCache);
            return app;
        }

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
                        // Signin using the ui required.
                        result = await App.AcquireTokenInteractive(AuthConfig.Config.Scopes).ExecuteAsync();
                    }
                }
                else
                {
                    result = await App.AcquireTokenInteractive(AuthConfig.Config.Scopes).ExecuteAsync();
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

        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", await GetAccessToken());
        }
    }
}
