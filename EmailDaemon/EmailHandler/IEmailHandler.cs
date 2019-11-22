using EmailDaemon.DataTypes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDaemon.EmailHandler
{
    public interface IEmailHandler
    {

        public Task InitialEmailsSyncAsync();

        public Task SyncEmailsAsync();

        public Task<IEnumerable<Email>> GetEmails();

    }
}
