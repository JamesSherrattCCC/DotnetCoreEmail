using EmailDaemon.Auth;
using EmailDaemon.EmailDatabase;
using EmailDaemon.Graph;
using System;
using System.Threading;

namespace EmailDaemon
{
    /// <summary>
    /// Program which syncs microsoft outlook email using Microsoft Graph and saves the results
    /// using EFCore.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new EmailContext())
            {
                // Create a new app this will also log the user in.
                var authApp = new AuthApp();
                // Create a new email handler for retrieving emails using MS Graph.
                var graphEmailClient = new GraphEmailHandler(authApp);
                // Database handler.
                var dbHandler = new EmailDbStorage(authApp.UserAccount.Username, db);
                // Handler for syncing emails with the database.
                var emailHandler = new EmailHandler.EmailHandler(graphEmailClient, dbHandler);
                Console.WriteLine("Syncing email database...");
                // Complete the initial sync with the database.
                emailHandler.InitialEmailsSyncAsync();
            
                while(! Console.KeyAvailable)
                {
                    Console.WriteLine("Updating email db...");
                    // Sync the emails until a key is pressed.
                    emailHandler.SyncEmailsAsync();
                    // Sleep so that the db is not updated more than every 30s.
                    // TODO: make this async.
                    Thread.Sleep(30000);
                }

            }

        }
    }
}
