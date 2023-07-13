using MailActivity;
using System.Activities;
using System.Diagnostics;
using System;


namespace MailActivity
{
    public class SendMailWindows : CodeActivity
    {
        public InArgument<string> To { get; set; }
        public InArgument<string> Cc { get; set; }
        public InArgument<string> Bcc { get; set; }
        public InArgument<string> Subject { get; set; }
        public InArgument<string> Body { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string to = To.Get(context);
            string cc = Cc.Get(context);
            string bcc = Bcc.Get(context);
            string subject = Subject.Get(context);
            string body = Body.Get(context);

            string mailtoUri = $"mailto:{to}?cc={cc}&bcc={bcc}&subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";
            Process.Start(mailtoUri);
        }
    }
}
