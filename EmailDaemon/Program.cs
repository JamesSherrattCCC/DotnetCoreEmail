using EmailDaemon.Auth;
using EmailDaemon.EmailDatabase;
using EmailDaemon.Graph;
using System;
using System.Threading;

namespace EmailDaemon
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EmailContext())
            {
                var authApp = new AuthApp();
                var graphEmailClient = new GraphEmailHandler(authApp);
                var dbHandler = new EmailDbStorage(authApp.UserAccount.Username, db);
                var emailHandler = new EmailHandler.EmailHandler(graphEmailClient, dbHandler);
                Console.WriteLine("Syncing email database...");
                emailHandler.InitialEmailsSyncAsync();
            
                while(! Console.KeyAvailable)
                {
                    Console.WriteLine("Updating email db...");
                    emailHandler.SyncEmailsAsync();
                    Thread.Sleep(30000);
                }

            }

        }
    }
}
