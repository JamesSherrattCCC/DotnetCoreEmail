using EmailDaemon.DataTypes;
using EmailDaemon.EmailHandler;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace EmailDaemon.Graph
{
    class GraphEmailHandler : IEmailRetriever
    {
        private GraphServiceClient _graphClient;

        private IMessageDeltaCollectionPage _curEmailPage;
        private IMessageDeltaRequest _nextEmailPageRequest;

        public GraphEmailHandler(IAuthenticationProvider authProvider)
        {
            _graphClient = new GraphServiceClient(authProvider);
        }

        public async Task<IEnumerable<Email>> GetAllEmails()
        {
            _nextEmailPageRequest = DeltaRequest();
            return await GetEmailsAsync();
        }

        public async Task<IEnumerable<Email>> GetEmailsFromDate(DateTimeOffset date)
        {
            _nextEmailPageRequest = _graphClient.Me.MailFolders.Inbox.Messages
                    .Delta()
                    .Request()
                    .Select("sender,subject,receivedDateTime,body")
                    .Filter($"ReceivedDateTime gt {date.UtcDateTime.ToString("O")}");
            return await GetEmailsAsync();
        }

        public async Task<IEnumerable<Email>> GetLatestEmails()
        {
            _nextEmailPageRequest = DeltaRequest();
            _nextEmailPageRequest.QueryOptions.Add(new QueryOption("$deltatoken", GetEmailDeltaToken()));
            return await GetEmailsAsync();
        }

        private string GetEmailDeltaToken()
        {
            object val;
            Uri deltaUri;
            _ = _curEmailPage.AdditionalData.TryGetValue("@odata.deltaLink", out val);
            deltaUri = new Uri(val as string);
            string deltaToken = HttpUtility.ParseQueryString(deltaUri.Query).Get("$deltatoken");
            return deltaToken;
        }

        private IMessageDeltaRequest DeltaRequest()
        {
            return _graphClient.Me.MailFolders.Inbox.Messages
                    .Delta()
                    .Request()
                    .Select("sender,subject,receivedDateTime,body")
                    .Filter("ReceivedDateTime ge 2019-01-01");
        }

        private async Task UpdatePages()
        {
            _curEmailPage = await _nextEmailPageRequest.GetAsync();
            _nextEmailPageRequest = _curEmailPage.NextPageRequest;
        }

        private async Task<IEnumerable<Email>> GetEmailsAsync()
        {
            try
            {
                await UpdatePages();
                IList<Email> emails = _curEmailPage.CurrentPage.ToEmails();

                while (_nextEmailPageRequest != null)
                {
                    _curEmailPage = await _nextEmailPageRequest.GetAsync();
                    foreach (Email email in _curEmailPage.ToEmails())
                    {
                        emails.Add(email);
                    }
                    _nextEmailPageRequest = _curEmailPage.NextPageRequest;
                }
                return emails;
            }
            catch (ServiceException ex)
            {
                Console.WriteLine("Error getting emails:");
                Console.WriteLine(ex);
                return null;
            }
        }

    }
}
