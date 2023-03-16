using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;
using System.Threading;

namespace Shortcuts
{
    public class Clipboard : CodeActivity
    {
        [Category("Input")] //type input
        [RequiredArgument] //to force the user to enter input
        public InArgument<string> InputText { get; set; }


        
       

        [STAThread]
        protected override void Execute(CodeActivityContext context)
        {
            // Get the input text from the activity property
            string input = InputText.Get(context);

            Thread thread = new Thread(() => System.Windows.Clipboard.SetText(input));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join(); //Wait for the thread to end

            
            Console.WriteLine("Text has been copied to clipboard.");
        }
    }
}
