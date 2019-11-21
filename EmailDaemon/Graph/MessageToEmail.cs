using EmailDaemon.DataTypes;
using Microsoft.Graph;
using System.Collections.Generic;

namespace EmailDaemon.Graph
{
    static class MessageToEmail
    {
        public static Email ToEmail(this Message message)
        {
            return new Email 
            { 
                Id = message.Id,
                Sender = message.Sender.EmailAddress.Address,
                Subject = message.Subject,
                DateRetrieved = message.ReceivedDateTime.Value,
                Body = message.Body.Content
            };
        }

        public static IEnumerable<Email> ToEmails(this IEnumerable<Message> messages)
        {
            IList<Email> emails = new List<Email>();
            foreach (Message message in messages)
            {
                emails.Add(message.ToEmail());
            }
            return emails;
        }

        public static IList<Email> ToEmails(this IList<Message> messages)
        {
            IList<Email> emails = new List<Email>();
            foreach (Message message in messages)
            {
                emails.Add(message.ToEmail());
            }
            return emails;
        }
    }
}
