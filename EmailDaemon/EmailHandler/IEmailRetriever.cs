using EmailDaemon.DataTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailDaemon.EmailHandler
{
    public interface IEmailRetriever
    {

        public Task<IEnumerable<Email>> GetAllEmails();

        public Task<IEnumerable<Email>> GetEmailsFromDate(DateTimeOffset date);

        public Task<IEnumerable<Email>> GetLatestEmails();

    }
}
