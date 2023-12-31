﻿using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel.Channels;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;

namespace MailActivity
{
    public class GetMailsWithIMAP : CodeActivity
    {
        [Category("Credentials")]
        [RequiredArgument]
        public InArgument<string> EmailAddress { get; set; }

        [Category("Credentials")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Category("Server")]
        public InArgument<string> Server { get; set; } = "imap.gmail.com"; // Default to Gmail server.

        [Category("Server")]
        public InArgument<int> Port { get; set; } = 993; // Default to Gmail IMAP port.

        [Category("Filters")]
        public InArgument<string> Folder { get; set; } = "INBOX"; // Default to INBOX folder.

        [Category("Filters")]
        public InArgument<int> MaxEmails { get; set; } = -1; // Default to fetch all emails.
        [Category("Options")]
        public bool MarkAsRead { get; set; }
        public OutArgument<List<MailMessage>> Mails { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string emailAddress = EmailAddress.Get(context);
            string password = Password.Get(context);
            string server = Server.Get(context);
            int port = Port.Get(context);
            string folder = Folder.Get(context);
            int maxEmails = MaxEmails.Get(context);
            bool markAsRead = MarkAsRead;

            List<MailMessage> mailList = new List<MailMessage>();

            // Connect to the mail server using IMAP.
            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine($"Connecting to {server} server...");
                    client.Connect(server, port, true);

                    // Enable SSL/TLS encryption.
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Tls;

                    Console.WriteLine("Logging in with provided credentials...");
                    // Log in with the provided credentials.
                    client.Authenticate(emailAddress, password);

                    // Select the specified folder.
                    var selectedFolder = client.GetFolder(folder);
                    selectedFolder.Open(MailKit.FolderAccess.ReadWrite); // Open the folder in read-write mode.

                    Console.WriteLine($"Fetching unread emails from {folder} folder...");

                    // Fetch only unread messages from the selected folder.
                    var uids = selectedFolder.Search(SearchQuery.NotSeen);
                    var messages = selectedFolder.Fetch(uids, MessageSummaryItems.UniqueId | MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure);

                    // Sort the messages by their date, in descending order (newest first).
                    var sortedMessages = messages.OrderByDescending(m => m.Date).Take(maxEmails == -1 ? messages.Count : maxEmails);

                    foreach (var message in sortedMessages)
                    {
                        var mimeMessage = selectedFolder.GetMessage(message.UniqueId);

                        // Convert the MimeMessage to a System.Net.Mail.MailMessage
                        MailMessage mailMessage = new MailMessage
                        {
                            Subject = mimeMessage.Subject,
                            From = new MailAddress(mimeMessage.From.ToString()),
                            Body = mimeMessage.TextBody
                        };

                        mailList.Add(mailMessage);

                        // Mark the message as "read" (Seen) to avoid fetching it again.
                        if (markAsRead)
                        {
                            selectedFolder.AddFlags(message.UniqueId, MessageFlags.Seen, true);
                        }
                    }

                    Console.WriteLine($"Fetched {mailList.Count} unread emails.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    // Disconnect from the server.
                    client.Disconnect(true);
                    Console.WriteLine("Disconnected from the server.");
                }
            }

            // Set the output argument to the list of mail messages.
            Mails.Set(context, mailList.Reverse<MailMessage>().ToList());
        }
    }
}