using System.Activities;

namespace MailActivity
{
    public class OpenWindowsMailApp : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            System.Diagnostics.Process.Start("mailto:");
        }
    }
}
