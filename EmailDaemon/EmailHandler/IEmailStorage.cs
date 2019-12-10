using EmailDaemon.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDaemon.EmailHandler
{
    /// <summary>
    /// Interface for saving emails to persistent storage.
    /// </summary>
    public interface IEmailStorage
    {        
        public Task SaveEmails(IEnumerable<Email> emails);

        public Task<IEnumerable<Email>> GetEmails();

        public Task<Email> GetLastRetrievedEmail();

    }
}
