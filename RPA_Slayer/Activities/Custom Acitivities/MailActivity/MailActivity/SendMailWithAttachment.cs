using System;
using System.Activities;
using System.Diagnostics;

namespace MailActivity
{
    public class SendMailWithAttachment : CodeActivity
    {
        public InArgument<string> To { get; set; }
        public InArgument<string> Cc { get; set; }
        public InArgument<string> Bcc { get; set; }
        public InArgument<string> Subject { get; set; }
        public InArgument<string> Body { get; set; }
        public InArgument<string> AttachmentPath { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string to = To.Get(context);
            string cc = Cc.Get(context);
            string bcc = Bcc.Get(context);
            string subject = Subject.Get(context);
            string body = Body.Get(context);
            string attachmentPath = AttachmentPath.Get(context);

            string mailtoUri = $"mailto:{to}?cc={cc}&bcc={bcc}&subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";

            if (!string.IsNullOrEmpty(attachmentPath))
            {
                mailtoUri += $"&attachment={Uri.EscapeDataString(attachmentPath)}";
            }

            Process.Start(mailtoUri);
        }
    }
}
