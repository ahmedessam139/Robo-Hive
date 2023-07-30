using System;
using System.Activities;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;

namespace MailActivity
{
    public class SendMailWithSMTP : CodeActivity
    {
        [Category("Credentials")]
        [RequiredArgument]
        public InArgument<string> EmailAddress { get; set; }

        [Category("Credentials")]
        [RequiredArgument]
        public InArgument<string> Password { get; set; }

        [Category("Server")]
        public InArgument<string> SmtpServer { get; set; } = "smtp.gmail.com"; // Default to Gmail SMTP server.

        [Category("Server")]
        public InArgument<int> SmtpPort { get; set; } = 587; // Default to Gmail SMTP port.

        [Category("Message")]
        [RequiredArgument]
        public InArgument<string> Recipient { get; set; }

        [Category("Message")]
        [RequiredArgument]
        public InArgument<string> Subject { get; set; }

        [Category("Message")]
        [RequiredArgument]
        public InArgument<string> Body { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string emailAddress = EmailAddress.Get(context);
            string password = Password.Get(context);
            string smtpServer = SmtpServer.Get(context);
            int smtpPort = SmtpPort.Get(context);
            string recipient = Recipient.Get(context);
            string subject = Subject.Get(context);
            string body = Body.Get(context);

            // Create the email message.
            MailMessage mail = new MailMessage(emailAddress, recipient)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Set this to true if the body contains HTML.
            };

            // Configure the SMTP client.
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailAddress, password),
                EnableSsl = true // Enable SSL/TLS encryption.
            };

            try
            {
                // Send the email.
                smtpClient.Send(mail);
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
            }
            finally
            {
                // Dispose of the resources.
                mail.Dispose();
                smtpClient.Dispose();
            }
        }
    }
}
