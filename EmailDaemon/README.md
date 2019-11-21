Email Daemon Project
-----------------------

This project lets a microsoft user:

1. Login to Microsoft via MSAL so they can access their email account via the Microsoft Graph API.
2. Update a local SQL server with emails.
3. Keep syncing the email inbox with new emails.

The service also saves the last delta token retrieved so that if the program is closed, it can continue from where it left off.