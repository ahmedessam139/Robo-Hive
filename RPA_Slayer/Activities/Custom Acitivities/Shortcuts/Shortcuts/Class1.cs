using System;
using System.Activities;
using System.Activities.Statements;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.ComponentModel;

namespace Shortcuts
{
    [System.ComponentModel.Designer(typeof(Clippy), typeof(System.ComponentModel.Design.IDesigner))]
    public sealed class KeyboardShortcut : CodeActivity
    {
        
        [Category("Input")] //type input
        [RequiredArgument] //to force the user to enter input
        public InArgument<string> TextToType { get; set; }


        [STAThread]
        protected override void Execute(CodeActivityContext context)
        {
            System.Threading.Thread.Sleep(1000);

            //var clippy = new Clippy();
            var selectedAction = context.GetValue(this.TextToType);
            
            if (selectedAction == "cut")
                {
                    System.Windows.Forms.SendKeys.SendWait("^x");
                }
                else if (selectedAction == "copy")
                {
                    System.Windows.Forms.SendKeys.SendWait("^c");
                }
                else if (selectedAction == "paste")
                {
                    System.Windows.Forms.SendKeys.SendWait("^v");
                }


           
        }

    }
}
