using EmailDaemon.Models;
using Microsoft.Graph;
using System.Collections.Generic;

namespace EmailDaemon.Graph
{
    /// <summary>
    /// Methods for converting a Microsoft graph message to the Email model defined in Models.
    /// </summary>
    static class MessageToEmail
    {
        /// <summary>
        /// Convert a message to an email.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The message represented as an email.</returns>
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

        /// <summary>
        /// Convert a list of MS Graph messages to a list of emails.
        /// </summary>
        /// <param name="messages">list of MS Graph messages.</param>
        /// <returns>list of emails.</returns>
        public static IEnumerable<Email> ToEmails(this IEnumerable<Message> messages)
        {
            IList<Email> emails = new List<Email>();
            foreach (Message message in messages)
            {
                emails.Add(message.ToEmail());
            }
            return emails;
        }

        /// <summary>
        /// Method for converting a list of MS Graph messages to a list of emails.
        /// </summary>
        /// <param name="messages">messages to convert</param>
        /// <returns>list of emails.</returns>
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
