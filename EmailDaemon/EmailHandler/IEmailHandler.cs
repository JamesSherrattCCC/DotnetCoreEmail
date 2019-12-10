using EmailDaemon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDaemon.EmailHandler
{
    /// <summary>
    /// Interface for syncing emails.
    /// </summary>
    public interface IEmailHandler
    {

        public Task InitialEmailsSyncAsync();

        public Task SyncEmailsAsync();

        public Task<IEnumerable<Email>> GetEmails();

    }
}
