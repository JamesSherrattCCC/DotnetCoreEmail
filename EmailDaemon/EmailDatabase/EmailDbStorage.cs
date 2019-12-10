using EmailDaemon.Models;
using EmailDaemon.EmailHandler;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace EmailDaemon.EmailDatabase
{
    /// <summary>
    /// Class for saving/ retrieving emails from EFCore persistent storage.
    /// </summary>
    class EmailDbStorage : IEmailStorage
    {

        private EmailContext _dbContext;
        private string _user;

        /// <summary>
        /// Create an email storage instance.
        /// </summary>
        /// <param name="user">The email user for which emails are being stored for.</param>
        /// <param name="ctx">Database context.</param>
        public EmailDbStorage(string user, EmailContext ctx)
        {
            _user = user;
            _dbContext = ctx;
        }

        /// <summary>
        /// Get emails from the database.
        /// </summary>
        /// <returns>Task which returns the emails in an IEnumerable.</returns>
        public async Task<IEnumerable<Email>> GetEmails()
        {
            return await _dbContext.Emails.Where(email => email.User == _user).ToListAsync();
        }

        /// <summary>
        /// Get the last email that was retrieved and stored in the Db.
        /// </summary>
        /// <returns>Task containing the email.</returns>
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

        /// <summary>
        /// Save an ienumerable of emails to the database.
        /// </summary>
        /// <param name="emails">IEnumerable of emails.</param>
        /// <returns></returns>
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
