using EmailDaemon.DataTypes;
using EmailDaemon.EmailHandler;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace EmailDaemon.EmailDatabase
{
    class EmailDbStorage : IEmailStorage
    {

        private EmailContext _dbContext;
        private string _user;

        public EmailDbStorage(string user, EmailContext ctx)
        {
            _user = user;
            _dbContext = ctx;
        }

        public async Task<IEnumerable<Email>> GetEmails()
        {
            return await _dbContext.Emails.Where(email => email.User == _user).ToListAsync();
        }

        public async Task<Email> GetLastRetrievedEmail()
        {
            var maxDate = await (from Date in _dbContext.Set<Email>()
                        where Date.User == _user
                        group Date by 1 into Dateg
                        select new
                        {
                            MaxDate = Dateg.Max(email => email.DateRetrieved)
                        }).FirstOrDefaultAsync();

            return await (from Mail in _dbContext.Set<Email>()
                                 where Mail.DateRetrieved == maxDate.MaxDate
                                 select Mail).FirstOrDefaultAsync();

        }

        public async Task SaveEmails(IEnumerable<Email> emails)
        {
            foreach (Email email in emails)
            {
                if (! await _dbContext.Emails.AnyAsync(e => e.Id == email.Id))
                {
                    email.User = _user;
                    await _dbContext.Emails.AddAsync(email);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
