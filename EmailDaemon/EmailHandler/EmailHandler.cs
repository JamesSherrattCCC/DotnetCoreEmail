using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EmailDaemon.DataTypes;

namespace EmailDaemon.EmailHandler
{
    class EmailHandler : IEmailHandler
    {

        private IEmailRetriever _emailRetriever;
        private IEmailStorage _emailStorage;
        private Email _lastEmail;

        public EmailHandler(IEmailRetriever retriever, IEmailStorage storage)
        {
            _emailRetriever = retriever;
            _emailStorage = storage;
            _lastEmail = _emailStorage.GetLastRetrievedEmail().Result;

        }

        public async Task<IEnumerable<Email>> GetEmails()
        {
            return await _emailStorage.GetEmails().ConfigureAwait(false);
        }

        public async Task InitialEmailsSyncAsync()
        {
            IEnumerable<Email> emails;

            if (_lastEmail != null)
            {
                Console.WriteLine($"Getting emails from {_lastEmail.DateRetrieved.UtcDateTime.ToString("O")}");
                emails = await _emailRetriever.GetEmailsFromDate(_lastEmail.DateRetrieved).ConfigureAwait(false);
            }
            else
            {
                emails = await _emailRetriever.GetAllEmails().ConfigureAwait(false);
            }

            if (emails != null)
                await _emailStorage.SaveEmails(emails).ConfigureAwait(false);
            else
                Console.WriteLine("Error retrieving emails.");
        }

        public async Task SyncEmailsAsync()
        {
            IEnumerable<Email> emails = await _emailRetriever.GetLatestEmails().ConfigureAwait(false);

            if (emails != null)
                await _emailStorage.SaveEmails(emails).ConfigureAwait(false);
            else
                Console.WriteLine("Error retrieving emails.");
        }

        public async Task SyncEmailsWithCheckAsync()
        {
            IEnumerable<Email> emails = await _emailRetriever.GetLatestEmails().ConfigureAwait(false);

            if (emails != null)
                await _emailStorage.SaveEmails(emails).ConfigureAwait(false);
            else
                Console.WriteLine("Error retrieving emails.");
        }
    }
}
