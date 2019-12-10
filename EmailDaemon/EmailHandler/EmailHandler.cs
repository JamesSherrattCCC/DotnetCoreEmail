using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmailDaemon.Models;

namespace EmailDaemon.EmailHandler
{
    /// <summary>
    /// Class which syncs, loads and saves emails.
    /// </summary>
    class EmailHandler : IEmailHandler
    {

        private IEmailRetriever _emailRetriever;
        private IEmailStorage _emailStorage;
        private Email _lastEmail;

        /// <summary>
        /// Create the email handler.
        /// </summary>
        /// <param name="retriever">Email retrieving object.</param>
        /// <param name="storage">Email storage object.</param>
        public EmailHandler(IEmailRetriever retriever, IEmailStorage storage)
        {
            _emailRetriever = retriever;
            _emailStorage = storage;
            _lastEmail = _emailStorage.GetLastRetrievedEmail().Result;

        }

        /// <summary>
        /// Get emails from the email server.
        /// </summary>
        /// <returns>IEnumerable containing emails.</returns>
        public async Task<IEnumerable<Email>> GetEmails()
        {
            return await _emailStorage.GetEmails();
        }

        /// <summary>
        /// Initial syncing of emails in the database.
        /// </summary>
        /// <returns>Task which syncs the emails.</returns>
        public async Task InitialEmailsSyncAsync()
        {
            IEnumerable<Email> emails;

            if (_lastEmail != null)
            {
                Console.WriteLine($"Getting emails from {_lastEmail.DateRetrieved.UtcDateTime.ToString("O")}");
                emails = await _emailRetriever.GetEmailsFromDate(_lastEmail.DateRetrieved);
            }
            else
            {
                emails = await _emailRetriever.GetAllEmails();
            }

            if (emails != null)
                await _emailStorage.SaveEmails(emails);
            else
                Console.WriteLine("Error retrieving emails.");
        }

        /// <summary>
        /// Sync the database with the email server.
        /// </summary>
        /// <returns>Task which syncs emails.</returns>
        public async Task SyncEmailsAsync()
        {
            IEnumerable<Email> emails = await _emailRetriever.GetLatestEmails();

            if (emails != null)
                await _emailStorage.SaveEmails(emails);
            else
                Console.WriteLine("Error retrieving emails.");
        }

        /// <summary>
        /// Check emails are not already in the database before adding them to the database.
        /// TODO: this is now no longer needed as the DBO checks this.
        /// </summary>
        /// <returns>Task which syncs the emails.</returns>
        public async Task SyncEmailsWithCheckAsync()
        {
            IEnumerable<Email> emails = await _emailRetriever.GetLatestEmails();

            if (emails != null)
                await _emailStorage.SaveEmails(emails);
            else
                Console.WriteLine("Error retrieving emails.");
        }
    }
}
