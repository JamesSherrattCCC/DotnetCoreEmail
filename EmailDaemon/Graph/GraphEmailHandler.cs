using EmailDaemon.Models;
using EmailDaemon.EmailHandler;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace EmailDaemon.Graph
{
    /// <summary>
    /// Class for retrieving emails from Microsoft Graph. This uses email
    /// deltas. See: https://docs.microsoft.com/en-us/graph/api/message-delta?view=graph-rest-1.0&tabs=http
    /// </summary>
    class GraphEmailHandler : IEmailRetriever
    {
        private GraphServiceClient _graphClient;

        private IMessageDeltaCollectionPage _curEmailPage;
        private IMessageDeltaRequest _nextEmailPageRequest;

        /// <summary>
        /// Create a MS Graph email handler when provided with an authorisation class
        /// (such as MSAL).
        /// </summary>
        /// <param name="authProvider">Authorisation provider (provides tokens) for MS Graph.</param>
        public GraphEmailHandler(IAuthenticationProvider authProvider)
        {
            _graphClient = new GraphServiceClient(authProvider);
        }

        /// <summary>
        /// Get all the unread emails (since Jam 2019) and mark them as read.
        /// </summary>
        /// <returns>Task which contains list of unread emails. </returns>
        public async Task<IEnumerable<Email>> GetAllEmails()
        {
            _nextEmailPageRequest = DeltaRequest();
            return await GetEmailsAsync();
        }

        /// <summary>
        /// Get unread emails from a date.
        /// </summary>
        /// <param name="date">date to retrieve emails from.</param>
        /// <returns>Task containing a list of emails from a date.</returns>
        public async Task<IEnumerable<Email>> GetEmailsFromDate(DateTimeOffset date)
        {
            _nextEmailPageRequest = _graphClient.Me.MailFolders.Inbox.Messages
                    .Delta()
                    .Request()
                    .Select("sender,subject,receivedDateTime,body,isread")
                    .Filter($"ReceivedDateTime gt {date.UtcDateTime.ToString("O")}");
            return await GetEmailsAsync();
        }

        /// <summary>
        /// Get the latest emails. This uses the delta token obtained from the last batch.
        /// </summary>
        /// <returns>Task containing list of the latest emails.</returns>
        public async Task<IEnumerable<Email>> GetLatestEmails()
        {
            _nextEmailPageRequest = DeltaRequest();
            _nextEmailPageRequest.QueryOptions.Add(new QueryOption("$deltatoken", GetEmailDeltaToken()));
            return await GetEmailsAsync();
        }

        /// <summary>
        /// Get the delta token for retrieving the next emails from the last set.
        /// </summary>
        /// <returns>String containing the delta token.</returns>
        private string GetEmailDeltaToken()
        {
            object val;
            Uri deltaUri;
            _ = _curEmailPage.AdditionalData.TryGetValue("@odata.deltaLink", out val);
            deltaUri = new Uri(val as string);
            string deltaToken = HttpUtility.ParseQueryString(deltaUri.Query).Get("$deltatoken");
            return deltaToken;
        }

        /// <summary>
        /// Get the next set of emails, using a delta token.
        /// </summary>
        /// <returns>The delta request.</returns>
        private IMessageDeltaRequest DeltaRequest()
        {
            return _graphClient.Me.MailFolders.Inbox.Messages
                    .Delta()
                    .Request()
                    .Select("sender,subject,receivedDateTime,body,isread")
                    .Filter("ReceivedDateTime ge 2019-01-01");
        }

        /// <summary>
        /// Update to the next set of pages.
        /// </summary>
        /// <returns>Task containing the next set of pages.</returns>
        private async Task UpdatePages()
        {
            _curEmailPage = await _nextEmailPageRequest.GetAsync();
            _nextEmailPageRequest = _curEmailPage.NextPageRequest;
        }

        /// <summary>
        /// Get the next set of emails async.
        /// </summary>
        /// <returns>Task which returns all the pages for the next set of emails.</returns>
        private async Task<IEnumerable<Email>> GetEmailsAsync()
        {
            try
            {
                await UpdatePages();
                IList<Email> emails = new List<Email>();
                ProcessMessages(ref emails);

                while (_nextEmailPageRequest != null)
                {
                    _curEmailPage = await _nextEmailPageRequest.GetAsync();
                    ProcessMessages(ref emails);
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

        /// <summary>
        /// Process the set of retrieved messages.
        /// For now, this just marks them as read.
        /// </summary>
        /// <param name="emails">list of emails to mark as read.</param>
        private void ProcessMessages(ref IList<Email> emails)
        {
            foreach (Message message in _curEmailPage)
            {
                if (message.IsRead == false)
                {
                    MarkAsRead(message);
                    emails.Add(message.ToEmail());
                }
            }
        }

        /// <summary>
        /// Mark the supplied email as read.
        /// </summary>
        /// <param name="msg">Email message to mark as read.</param>
        /// <returns>Task which marks the email as read.</returns>
        private async Task MarkAsRead(Message msg)
        {
            var messageUpdate = new Message
            {
                IsRead = true
            };
            await _graphClient.Me.Messages[msg.Id].Request().Select("IsRead").UpdateAsync(messageUpdate);
        }
    }
}
