using EmailDaemon.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDaemon.EmailHandler
{
    public interface IEmailStorage
    {        
        public Task SaveEmails(IEnumerable<Email> emails);

        public Task<IEnumerable<Email>> GetEmails();

        public Task<Email> GetLastRetrievedEmail();

    }
}
